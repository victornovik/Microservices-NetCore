# ASP.NET Core Microservices

## Display all available project templates
```
dotnet new list
```

## Create ASP.NET Core project
```
dotnet new webapi -n Play.Catalog.Service
dotnet dev-certs https --trust
```

## Build project (Go to Play.Catalog\src\Play.Catalog.Service)
```
dotnet build
```

## Run project
```
dotnet run
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