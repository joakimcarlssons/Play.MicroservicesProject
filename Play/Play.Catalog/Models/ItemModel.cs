using Play.Common;

namespace Play.Catalog.Models
{
    public class ItemModel : IEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [Range(0, 1000)]
        public decimal Price { get; set; }


        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
