using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterConfiguration entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterConfigurationMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigurationMappingProfile"/> class.
        /// </summary>
        public RequesterConfigurationMappingProfile()
        {
            CreateMap<RequesterConfiguration, RequesterConfigurationDto>().ReverseMap();
        }
    }
}

