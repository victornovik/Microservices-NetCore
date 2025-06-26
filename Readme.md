# ASP.NET Core Microservices

## Display all available project templates
```
dotnet new list
```

## Create project, build, run
```
dotnet new webapi -n Play.Catalog.Service
dotnet new classlib -n Play.Common

dotnet dev-certs https --trust
dotnet build
dotnet run
```

## Create NuGet package from Play.Common project
```
dotnet pack -o ..\..\..\packages
```

## Add local NuGet packages storage
```
dotnet nuget add source D:\dev\src\casal_microservices\packages -n PlayEconomy
```

## Add reference to Play.Common NuGet package
```
dotnet add package Play.Common
```

## MongoDB setup
```
dotnet add package MongoDB.Driver

docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
docker ps
```

### EF dependencies
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```
### Create an initial migration
```
dotnet ef migrations add initial
dotnet ef migrations add bids
```
### Apply the initial migration to the database
```
dotnet ef database update
```