#!/bin/bash
#
# Adds the .NET Core 1.0.3 SDK to the current container and builds.
# To be run within the microsoft/dotnet:sdk container to build datadiluvium.
# docker run -v `pwd`:/src --rm -it microsoft/dotnet:sdk /bin/bash -c "cd /src; ./build-in-docker.sh"
#
curl -SL https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.3/dotnet-debian-x64.1.0.3.tar.gz --output dotnet.tar.gz
tar -xzf dotnet.tar.gz -C /usr/share/dotnet/
rm dotnet.tar.gz
cd /src
dotnet restore
dotnet publish -c Release -r debian.8-x64 -o release
rm -rf bin/ obj/ project.lock.json
