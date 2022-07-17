using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Clients;

namespace Play.Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        #region Private Members

        private readonly IRepository<InventoryItem> _inventoryItemsRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        #endregion

        #region Constructor

        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IMapper mapper, IRepository<CatalogItem> catalogItemRepository)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogItemRepository = catalogItemRepository;
            _mapper = mapper;
        }

        #endregion

        #region Endpoints

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId).ToList();
            var catalogItemEntities = await _catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

                var inventoryItemDto = _mapper.Map<InventoryItemDto>(catalogItem);
                inventoryItemDto.Quantity = inventoryItem.Quantity;
                inventoryItemDto.AcquiredDate = inventoryItem.AcquiredDate;
                return inventoryItemDto;
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await _inventoryItemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await _inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }

        #endregion
    }
}
