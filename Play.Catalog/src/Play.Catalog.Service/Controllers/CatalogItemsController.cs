using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class CatalogItemsController(IRepository<Entity> repository) : ControllerBase
{
    private static int requests;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        var curRequest = ++requests;
        Console.WriteLine($"{DateTimeOffset.UtcNow}> Request {curRequest} starting");
        
        if (curRequest < 3)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow}> Request {curRequest} delaying");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        if (curRequest < 5)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow}> Request {curRequest} returning 500");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var dtos = (await repository.GetAllAsync()).Select(item => item.AsDto());

        Console.WriteLine($"{DateTimeOffset.UtcNow}> Request {curRequest} returning 200");

        return Ok(dtos);
    }

    // GET /items/12345
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await repository.GetAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item.AsDto());
    }

    // POST /items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto dto)
    {
        var item = new Entity{Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description, Price = dto.Price, CreatedDate = DateTimeOffset.UtcNow};
        await repository.CreateAsync(item);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    // PUT /items/12345
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto dto)
    {
        var item = await repository.GetAsync(id);
        if (item == null)
            return NotFound();

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;

        await repository.UpdateAsync(item);
        return NoContent();
    }

    // DELETE /items/12345
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await repository.GetAsync(id);
        if (item == null)
            return NotFound();

        await repository.DeleteAsync(id);
        return NoContent();
    }
}