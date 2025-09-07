FROM ubuntu:24.04

# Install MySQL Shell
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y mysql-shell && \
    apt-get install -y mysql-client && \
    rm -rf /var/lib/apt/lists/*

# Install locale
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y locales && \
    locale-gen en_US.UTF-8 && \
    update-locale LANG=en_US.UTF-8 && \
    rm -rf /var/lib/apt/lists/*

# Copy the entrypoint script
COPY ./docker-entrypoint.bash /docker-entrypoint.bash
RUN chmod +x /docker-entrypoint.bash

# Start MySQL Shell
ENTRYPOINT ["/docker-entrypoint.bash"]
