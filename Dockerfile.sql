FROM mcr.microsoft.com/mssql/server:2022-latest
USER root
RUN apt-get update && apt-get install -y gcc && rm -rf /var/lib/apt/lists/*
COPY fake_ram.c /tmp/fake_ram.c
RUN gcc -shared -fPIC /tmp/fake_ram.c -o /usr/lib/libfake_ram.so && rm /tmp/fake_ram.c
ENV LD_PRELOAD=/usr/lib/libfake_ram.so
USER mssql
