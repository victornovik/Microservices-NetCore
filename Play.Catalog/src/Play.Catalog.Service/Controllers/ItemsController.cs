using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items =
        [
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword", "Hits a small amount of damage", 20, DateTimeOffset.UtcNow)
        ];

        [HttpGet]
        public IEnumerable<ItemDto> Get() => items;

        // GET /items/12345
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var cur = items.SingleOrDefault(i => i.Id == id);
            if (cur == null)
                return NotFound();
            return cur;
        }

        // POST /items
        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto dto)
        {
            var created = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            items.Add(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /items/12345
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto dto)
        {
            var cur = items.SingleOrDefault(i => i.Id == id);
            if (cur == null)
                return NotFound();

            var updated = cur with
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            var index = items.FindIndex(item => item.Id == id);
            items[index] = updated;
            return NoContent();
        }

        // DELETE /items/12345
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var removed = items.RemoveAll(item => item.Id == id);
            if (removed == 0)
                return NotFound();
            return NoContent();
        }
    }
}