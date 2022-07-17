using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IRepository<ItemModel> _repo;
        private readonly IPublishEndpoint _publishEndpoint; 

        #endregion

        #region Constructor

        public ItemsController(IMapper mapper, IRepository<ItemModel> repo, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _repo = repo;
            _publishEndpoint = publishEndpoint;
        }

        #endregion

        #region Endpoints

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetAllItems()
        {
            var items = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ItemReadDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemReadDto>> GetItemById(Guid id)
        {
            Console.WriteLine($"--> Trying to get item with id { id }");


            var item = await _repo.GetAsync(id);
            if(item != null)
            {
                Console.WriteLine($"--> Found item with id { id }");
                return Ok(_mapper.Map<ItemReadDto>(item));
            }
            else
            {
                Console.WriteLine($"--> Could not find item with id { id }");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemReadDto>> CreateItem(ItemCreateDto itemCreateDto)
        {
            var item = _mapper.Map<ItemModel>(itemCreateDto);

            // Create item
            await _repo.CreateAsync(item);
            Console.WriteLine($"--> New item was created: { item.Id }");

            // Publish created item
            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            Console.WriteLine($"--> New item was published: { item.Id }");

            return CreatedAtAction(nameof(GetItemById), new { id = Guid.NewGuid() }, item);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, ItemUpdateDto itemUpdateDto)
        {
            Console.WriteLine($"--> Updating item { id }");

            var existingItem = await _repo.GetAsync(id);
            if (existingItem == null)
            {
                Console.WriteLine($"--> Could not find item to update: { id }");
                return NotFound();
            }

            _mapper.Map(itemUpdateDto, existingItem);

            // Update item
            await _repo.UpdateAsync(existingItem);
            Console.WriteLine($"--> Item was updated: { id }");

            // Publish updated item
            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));
            Console.WriteLine($"--> New item was published because of update: { existingItem.Id }");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            var item = await _repo.GetAsync(id);
            if (item == null)
            {
                Console.WriteLine($"--> Could not find item to delete: { id }");
                return NotFound();
            }

            // Delete item
            await _repo.RemoveAsync(item.Id);
            Console.WriteLine($"--> Item was deleted: { item.Id }");

            // Publish deleted item
            await _publishEndpoint.Publish(new CatalogItemDeleted(item.Id));
            Console.WriteLine($"--> New item was published because of deletion: { item.Id }");

            return NoContent();
        }

        #endregion
    }
}
