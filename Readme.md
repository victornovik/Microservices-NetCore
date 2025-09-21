# ASP.NET Core Microservices

## Display all available project templates
```powershell
dotnet new list
```

## Create project, build, run
```powershell
dotnet new webapi -n Play.Catalog.Service
dotnet new classlib -n Play.Common
dotnet new classlib -n Play.Catalog.Contracts
dotnet add reference ..\Play.Catalog.Contracts\Play.Catalog.Contracts.csproj

dotnet dev-certs https --trust
dotnet build
dotnet run --launch-profile https
```

## Create NuGet package from Play.Common project
```powershell
dotnet pack -o ..\..\..\packages

# Repack new changes into the package and uplift the version
dotnet pack -p:PackageVersion=1.0.1 -o ..\..\..\packages\

# Add local NuGet packages storage
dotnet nuget add source D:\dev\src\casal_microservices\packages -n PlayEconomy
```

## Add reference to Play.Common NuGet package
```powershell
dotnet add package Play.Common
```

## MongoDB
```powershell
dotnet add package MongoDB.Driver
```

## Polly
```powershell
dotnet add package Microsoft.Extensions.Http.Polly
```

## RabbitMQ
```powershell
dotnet add package MassTransit.RabbitMQ
docker pull rabbitmq:4.1.1-management

# See messages in browser
http://localhost:15672/#/exchanges
```

## Docker
```powershell
docker-compose up -d
docker ps
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
docker stop mongo
```

## EF dependencies
```powershell
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Create an initial migration
```powershell
dotnet ef migrations add initial
dotnet ef migrations add bids
```

## Apply the initial migration to the database
```powershell
dotnet ef database update
```