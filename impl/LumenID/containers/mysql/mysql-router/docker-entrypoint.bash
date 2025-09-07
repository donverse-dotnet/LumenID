#!/bin/bash

# Get environment variables and if not set, exit with error
: "${MYSQL_ROUTER_USER:?Environment variable MYSQL_ROUTER_USER not set}"
: "${MYSQL_ROUTER_PASSWORD:?Environment variable MYSQL_ROUTER_PASSWORD not set}"
: "${MYSQL_ROUTER_HOST:?Environment variable MYSQL_ROUTER_HOST not set}"
: "${MYSQL_ROUTER_PORT:=3306}"

# Awaiting for MySQL server to be available
echo "Waiting for MySQL server at $MYSQL_ROUTER_HOST:$MYSQL_ROUTER_PORT to be available..."
until mysqladmin ping -h "$MYSQL_ROUTER_HOST" -P "$MYSQL_ROUTER_PORT" -u "$MYSQL_ROUTER_USER" -p"$MYSQL_ROUTER_PASSWORD" --silent; do
    echo "MySQL server is not available yet. Retrying in 5 seconds..."
    sleep 5
done
echo "MySQL server is available."

# Create mysqlrouter user if not exists
echo "Creating system user 'mysqlrouter'..."
if ! id -u mysqlrouter >/dev/null 2>&1; then
    useradd -r -s /bin/false mysqlrouter
    echo "User 'mysqlrouter' created."
else
    echo "User 'mysqlrouter' already exists."
fi

# Awaiting for MySQL Shell job to be done
echo "Waiting for MySQL Shell job to be done..."
sleep 70
echo "Assuming MySQL Shell job is done."

# Configure MySQL Router
echo "Configuring MySQL Router..."
mysqlrouter --bootstrap $MYSQL_ROUTER_USER:$MYSQL_ROUTER_PASSWORD@$MYSQL_ROUTER_HOST:$MYSQL_ROUTER_PORT --user=mysqlrouter --directory /var/lib/mysqlrouter --conf-use-sockets

# Start mysql router
echo "Starting MySQL Router..."
mysqlrouter --user=mysqlrouter --config /var/lib/mysqlrouter/mysqlrouter.conf
