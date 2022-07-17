using MassTransit;
using Play.Common;
using static Play.Catalog.Contracts.Contracts;

namespace Play.Inventory.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        #region Private Members

        private readonly IRepository<CatalogItem> _repo;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repo, IMapper mapper)
        {
            _repo = repo;
        }

        #endregion

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            // Get the message
            var message = context.Message;

            // Verify that item exists
            var item = await _repo.GetAsync(message.ItemId);
            if (item == null) { return; } // If the item does not exist, do nothing

            // If the item exists, delete it
            await _repo.RemoveAsync(message.ItemId);
        }
    }
}
