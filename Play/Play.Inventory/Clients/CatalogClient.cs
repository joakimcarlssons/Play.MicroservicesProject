namespace Play.Inventory.Clients
{
    public class CatalogClient
    {
        #region Private Members

        private readonly HttpClient _httpClient;

        #endregion

        #region Constructor

        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        public async Task<IReadOnlyCollection<CatalogItemReadDto>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemReadDto>>("/api/items");
            return items;
        }
    }
}
