using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entitites;

namespace Play.Inventory.Service;

public static class Extensions
{
    // Convert inventory item to DTO exposed outside
    public static InventoryItemDto AsDto(this InventoryEntity entity)
    {
        return new InventoryItemDto(entity.CatalogItemId, entity.Quantity, entity.AcquiredDate);
    }
}