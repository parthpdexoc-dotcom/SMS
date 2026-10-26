using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterConfig entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterConfigMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigMappingProfile"/> class.
        /// </summary>
        public RequesterConfigMappingProfile()
        {
            CreateMap<RequesterConfig, RequesterConfigDto>().ReverseMap();
        }
    }
}
