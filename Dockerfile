FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ADD ./src /code
WORKDIR /code

RUN mkdir /artifacts

RUN dotnet restore  --configfile ./NuGet.Config
    
RUN dotnet publish -c Release -o /artifacts

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
# RUN apt-get update && apt-get -y install ca-certificates && update-ca-certificates
RUN apt-get update && apt-get install -y vim
RUN apt install -y grep mlocate
WORKDIR /app
COPY --from=build /artifacts .
ENTRYPOINT ["dotnet", "RabbitMQLib.dll"]