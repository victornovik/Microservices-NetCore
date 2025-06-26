using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service;

public static class Extensions
{
    // Convert Repository item to DTO exposed outside
    public static ItemDto AsDto(this Entity entity)
    {
        return new ItemDto(entity.Id, entity.Name, entity.Description, entity.Price, entity.CreatedDate);
    }
}