FROM ubuntu:24.04

# Install MySQL Router
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y mysql-router && \
    apt-get install -y mysql-client && \
    rm -rf /var/lib/apt/lists/*

# Expose the default MySQL Router port
EXPOSE 7001

# Copy the entrypoint script
COPY ./docker-entrypoint.bash /docker-entrypoint.bash
RUN chmod +x /docker-entrypoint.bash

# Start MySQL Router
ENTRYPOINT ["/docker-entrypoint.bash"]
