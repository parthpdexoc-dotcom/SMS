using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class PostCallSignMappingProfile : Profile
    {
        public PostCallSignMappingProfile()
        {
            CreateMap<PostCallSign, PostCallSignDto>().ReverseMap();
        }
    }
}
