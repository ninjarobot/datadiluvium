FROM microsoft/dotnet:runtime-deps
MAINTAINER Data Diluvium
ADD ./release/* /usr/local/datadiluvium/
EXPOSE 8080
ENV LISTEN_PORT = "8080"
ENTRYPOINT ["/usr/local/datadiluvium/datadiluvium"]