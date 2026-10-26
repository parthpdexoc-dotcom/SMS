using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class RequestTypeMappingProfile : Profile
    {
        public RequestTypeMappingProfile()
        {
            CreateMap<RequestType, RequestTypeDto>()
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.Type))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.RequestType));
        }
    }
}
