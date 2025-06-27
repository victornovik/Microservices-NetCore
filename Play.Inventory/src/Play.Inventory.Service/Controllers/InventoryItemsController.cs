using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entitites;
using Play.Inventory.Service.HttpClients;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class InventoryItemsController(IRepository<InventoryEntity> repository, CatalogClient catalogClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var catalogItems = await catalogClient.GetCatalogItemsAsync();
        var inventoryItems = await repository.GetAllAsync(i => i.UserId == userId);
        var inventoryItemDtos = inventoryItems.Select(ii =>
        {
            var catalogItem = catalogItems.Single(ci => ci.Id == ii.CatalogItemId);
            return ii.AsDto(catalogItem.Name, catalogItem.Description);
        });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto dto)
    {
        var item = await repository.GetAsync(i => i.UserId == dto.UserId && i.CatalogItemId == dto.CatalogItemId);
        if (item == null)
        {
            item = new InventoryEntity { UserId = dto.UserId, CatalogItemId = dto.CatalogItemId, Quantity = dto.Quantity, AcquiredDate = DateTimeOffset.UtcNow };
            await repository.CreateAsync(item);
        }
        else
        {
            item.Quantity += dto.Quantity;
            await repository.UpdateAsync(item);
        }
        return Ok();
    }
}