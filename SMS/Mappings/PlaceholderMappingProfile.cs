using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class PlaceholderMappingProfile : Profile
    {
        public PlaceholderMappingProfile()
        {
            CreateMap<BulkAssignPlaceholderItemDto, PlaceholderDto>();
            CreateMap<Placeholder, PlaceholderDto>();
            CreateMap<PlaceholderDto, Placeholder>();
        }
    }
}
