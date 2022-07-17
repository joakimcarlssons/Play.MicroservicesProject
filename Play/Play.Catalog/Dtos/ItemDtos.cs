using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Dtos
{
    public record ItemReadDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate, DateTimeOffset LastUpdated);
    public record ItemCreateDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);
    public record ItemUpdateDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);
}
