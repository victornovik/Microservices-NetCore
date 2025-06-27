using System.ComponentModel.DataAnnotations;

namespace Play.Inventory.Service.Dtos;

public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
public record InventoryItemDto(Guid CatalogItemId, int Qty, DateTimeOffset AcquiredDate);
