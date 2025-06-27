using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entitites;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class InventoryItemsController(IRepository<InventoryEntity> repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var items = (await repository.GetAllAsync(i => i.UserId == userId))
            .Select(i => i.AsDto());
        return Ok(items);
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