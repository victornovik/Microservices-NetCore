using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entitites;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemUpdatedConsumer(IRepository<CatalogEntity> repository) : IConsumer<CatalogItemUpdated>
{
    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var msg = context.Message;
        var entity = new CatalogEntity { Id = msg.ItemId, Name = msg.Name, Description = msg.Description };

        if (await repository.GetAsync(msg.ItemId) == null)
            await repository.CreateAsync(entity);
        else
            await repository.UpdateAsync(entity);
    }
}