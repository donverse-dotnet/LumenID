FROM ubuntu:24.04

# Install HAProxy
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y haproxy && \
    rm -rf /var/lib/apt/lists/*

# Expose the default HAProxy port
EXPOSE 3306
