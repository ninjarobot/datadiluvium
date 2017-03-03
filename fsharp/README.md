Datadiluvium FSharp Implementation
========

A service for generating random data and SQL scripts on request.

Implementation is in F# and depends on .NET Core to build.

From a machine with the .NET Core RC4 or later installed, execute the following
to restore packages, build, and execute tests.

    dotnet restore && dotnet build && dotnet test datadiluvium.tests/datadiluvium.tests.fsproj

To run the project use the `dotnet run` command.

    dotnet run --project datadiluvium/datadiluvium.fsproj

Both commands can be run easily using the official `microsoft/dotnet` docker 
image for the `sdk`.  The source should be mounted from this directory.

    docker run -v `pwd`:/src -p 8080:8080 --rm -i microsoft/dotnet:sdk /bin/bash -c "cd src; dotnet restore && dotnet run --project datadiluvium/datadiluvium.fsproj"

After running the command above, the service should start listening on
container port 8080, which is also mapped to the host's port 8080.  Try the
service with an HTTP request to http://{host}:8080/random/city/en.