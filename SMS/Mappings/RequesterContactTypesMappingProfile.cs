using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterContactTypes entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterContactTypesMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactTypesMappingProfile"/> class.
        /// </summary>
        public RequesterContactTypesMappingProfile()
        {
            CreateMap<RequesterContactTypes, RequesterContactTypesDto>().ReverseMap();
        }
    }
}

