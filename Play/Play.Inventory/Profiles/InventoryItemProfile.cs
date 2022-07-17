using static Play.Catalog.Contracts.Contracts;

namespace Play.Inventory.Profiles
{
    public class InventoryItemProfile : Profile
    {
        public InventoryItemProfile()
        {
            // Source -> Target
            CreateMap<InventoryItem, InventoryItemDto>();
            CreateMap<CatalogItemReadDto, InventoryItemDto>();
            CreateMap<CatalogItemCreated, CatalogItem>();
            CreateMap<CatalogItemUpdated, CatalogItem>();

        }
    }
}
