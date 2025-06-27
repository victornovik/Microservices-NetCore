using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.HttpClients;

public class CatalogClient(HttpClient httpClient)
{
    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
        return items;
    }
}