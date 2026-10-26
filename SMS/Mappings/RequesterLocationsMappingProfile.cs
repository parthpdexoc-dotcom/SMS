using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterLocations entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterLocationsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterLocationsMappingProfile"/> class.
        /// </summary>
        public RequesterLocationsMappingProfile()
        {
            CreateMap<RequesterLocations, RequesterLocationsDto>().ReverseMap();
        }
    }
}

