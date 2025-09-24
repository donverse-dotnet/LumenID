FROM mysql:8.0.36

# Copy custom configuration file
COPY ./server-02.cnf /etc/mysql/conf.d/server-02.cnf

# Change permissions for the configuration file
RUN chmod 644 /etc/mysql/conf.d/server-02.cnf && \
    chown mysql:mysql /etc/mysql/conf.d/server-02.cnf
