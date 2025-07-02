using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entitites;
using Play.Inventory.Service.HttpClients;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class InventoryItemsController(IRepository<InventoryEntity> inventoryRepository, IRepository<CatalogEntity> catalogRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        // REST request to fetch all catalog items from Play.Catalog.Service
        // var catalogItems = await catalogClient.GetCatalogItemsAsync();

        var inventoryItems = await inventoryRepository.GetAllAsync(ie => ie.UserId == userId);
        var inventoryItemsIds = inventoryItems.Select(ie => ie.CatalogItemId);

        // Local MongoDB request to fetch only those catalog items that belong to the user
        var catalogItems = await catalogRepository.GetAllAsync(ce => inventoryItemsIds.Contains(ce.Id));
        
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
        var item = await inventoryRepository.GetAsync(i => i.UserId == dto.UserId && i.CatalogItemId == dto.CatalogItemId);
        if (item == null)
        {
            item = new InventoryEntity { UserId = dto.UserId, CatalogItemId = dto.CatalogItemId, Quantity = dto.Quantity, AcquiredDate = DateTimeOffset.UtcNow };
            await inventoryRepository.CreateAsync(item);
        }
        else
        {
            item.Quantity += dto.Quantity;
            await inventoryRepository.UpdateAsync(item);
        }
        return Ok();
    }
}