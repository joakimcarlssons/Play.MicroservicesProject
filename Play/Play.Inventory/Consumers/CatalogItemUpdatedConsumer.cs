using MassTransit;
using Play.Common;
using static Play.Catalog.Contracts.Contracts;

namespace Play.Inventory.Consumers
{
    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
    {
        #region Private Members

        private readonly IRepository<CatalogItem> _repo;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        #endregion

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            // Get the message
            var message = context.Message;

            // Verify that the item exists
            var item = await _repo.GetAsync(message.ItemId);
            if (item == null)
            {
                // If not, we create the item
                await _repo.CreateAsync(_mapper.Map<CatalogItem>(message));
            }
            else
            {
                // If it exists we update
                await _repo.UpdateAsync(_mapper.Map<CatalogItem>(message));
            }
        }
    }
}
