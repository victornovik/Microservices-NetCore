# ASP.NET Core Microservices

## Display all available project templates
```
dotnet new list
```

## Create project, build, run
```
dotnet new webapi -n Play.Catalog.Service
dotnet dev-certs https --trust

dotnet build

dotnet run
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