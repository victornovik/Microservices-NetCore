using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        // Needed for GUID serializing since MongoDB.Driver 3.x
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Save GUID and DateTimeOffset as string
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        
        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var cfg = serviceProvider.GetService<IConfiguration>();
            if (cfg == null)
                ArgumentNullException.ThrowIfNull(cfg);
            
            var mongoDbSettings = cfg.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            if (mongoDbSettings == null)
                ArgumentNullException.ThrowIfNull(mongoDbSettings);
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

            var serviceSettings = cfg.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            if (serviceSettings == null)
                ArgumentNullException.ThrowIfNull(serviceSettings);
            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });
        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) 
        where T: IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            if (database == null)
                ArgumentNullException.ThrowIfNull(database);

            return new MongoRepository<T>(database, collectionName);
        });
        return services;
    }
}