using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository
{
    private const string collectionName = "items";
    private readonly IMongoCollection<Item> collection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    static ItemsRepository()
    {
        // Needed for GUID serializing since MongoDB.Driver 3.x
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Save GUID and DateTimeOffset as string
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    }

    public ItemsRepository()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("Catalog");
        collection = database.GetCollection<Item>(collectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {
        return await collection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<Item> GetAsync(Guid id)
    {
        var filter = filterBuilder.Eq(e => e.Id, id);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        await collection.InsertOneAsync(item);
    }

    public async Task UpdateAsync(Item item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        var filter = filterBuilder.Eq(e => e.Id, item.Id);
        await collection.ReplaceOneAsync(filter, item);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = filterBuilder.Eq(e => e.Id, id);
        await collection.DeleteOneAsync(filter);
    }
}