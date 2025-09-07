#!/bin/bash

# Configure Innodb with mysqlsh
# Separate servers by commas
IFS=',' read -r -a SERVERS_ARRAY <<< "$MYSQL_SERVERS"

# Get environment for mysql user and password
: "${MYSQL_USER:?Environment variable MYSQL_USER not set}"
: "${MYSQL_PASSWORD:?Environment variable MYSQL_PASSWORD not set}"

# Awaiting for MySQL servers to be available
for SERVER in "${SERVERS_ARRAY[@]}"; do
    echo "Waiting for MySQL server at $SERVER to be available..."
    until mysqladmin ping -h "$SERVER" -P 3306 -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" --silent; do
        echo "MySQL server at $SERVER is not available yet. Retrying in 5 seconds..."
        sleep 5
    done
    echo "MySQL server at $SERVER is available."
done

# Configure InnoDB Cluster
echo "Configuring InnoDB Cluster..."
mysqlsh --version
for SERVER in "${SERVERS_ARRAY[@]}"; do
    echo "$MYSQL_USER:$MYSQL_PASSWORD@$SERVER"
    mysqlsh --python -e "dba.configure_instance('$MYSQL_USER:$MYSQL_PASSWORD@$SERVER'); exit()"
done

# Create the cluster
PRIMARY_SERVER="${SERVERS_ARRAY[0]}"
echo "Creating InnoDB Cluster on primary server $PRIMARY_SERVER..."
mysqlsh --python --uri $MYSQL_USER:$MYSQL_PASSWORD@$PRIMARY_SERVER -e "cluster = dba.create_cluster('myCluster'); exit()"
# Add instances to the cluster
for SERVER in "${SERVERS_ARRAY[@]:1}"; do
    echo "Adding instance $SERVER to the cluster..."
    mysqlsh --python --uri $MYSQL_USER:$MYSQL_PASSWORD@$PRIMARY_SERVER -e "cluster = dba.get_cluster('myCluster'); cluster.add_instance('$MYSQL_USER:$MYSQL_PASSWORD@$SERVER', {'recoveryMethod':'clone'}); exit()"
done
