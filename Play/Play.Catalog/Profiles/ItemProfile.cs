using System;

namespace Play.Catalog.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            // Source -> Target
            CreateMap<ItemModel, ItemReadDto>();
            CreateMap<ItemCreateDto, ItemModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<ItemUpdateDto, ItemModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
        }
    }
}
