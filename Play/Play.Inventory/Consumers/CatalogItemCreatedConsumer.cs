using MassTransit;
using Play.Common;
using static Play.Catalog.Contracts.Contracts;

namespace Play.Inventory.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        #region Private Members

        private readonly IRepository<CatalogItem> _repo;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        #endregion

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            // Get the message
            var message = context.Message;

            // Verify that item is not already created
            var item = await _repo.GetAsync(message.ItemId);
            if (item != null) { return; }

            await _repo.CreateAsync(_mapper.Map<CatalogItem>(message));
        }
    }
}
