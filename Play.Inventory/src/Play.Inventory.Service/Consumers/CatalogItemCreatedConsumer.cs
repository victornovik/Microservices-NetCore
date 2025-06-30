using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entitites;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer(IRepository<CatalogEntity> repository) : IConsumer<CatalogItemCreated>
{
    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var msg = context.Message;

        var entity = await repository.GetAsync(msg.ItemId);
        if (entity != null)
            return;

        entity = new CatalogEntity { Id = msg.ItemId, Name = msg.Name, Description = msg.Description };
        await repository.CreateAsync(entity);
    }
}