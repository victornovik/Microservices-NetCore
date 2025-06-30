using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entitites;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemDeletedConsumer(IRepository<CatalogEntity> repository) : IConsumer<CatalogItemDeleted>
{
    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var msg = context.Message;
        await repository.DeleteAsync(msg.ItemId);
    }
}