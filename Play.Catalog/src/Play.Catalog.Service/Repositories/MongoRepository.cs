using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> collection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        collection = database.GetCollection<T>(collectionName);
    }

    static MongoRepository()
    {
        // Needed for GUID serializing since MongoDB.Driver 3.x
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Save GUID and DateTimeOffset as string
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await collection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        var filter = filterBuilder.Eq(e => e.Id, id);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var filter = filterBuilder.Eq(e => e.Id, entity.Id);
        await collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = filterBuilder.Eq(e => e.Id, id);
        await collection.DeleteOneAsync(filter);
    }
}