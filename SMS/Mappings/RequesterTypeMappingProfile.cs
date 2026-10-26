using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterType entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterTypeMappingProfile"/> class.
        /// </summary>
        public RequesterTypeMappingProfile()
        {
            CreateMap<RequesterType, RequesterTypeDto>().ReverseMap();
        }
    }
}

