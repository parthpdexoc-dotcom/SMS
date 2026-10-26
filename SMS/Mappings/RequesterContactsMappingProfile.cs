using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterContacts entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterContactsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactsMappingProfile"/> class.
        /// </summary>
        public RequesterContactsMappingProfile()
        {
            CreateMap<RequesterContacts, RequesterContactsDto>().ReverseMap();
        }
    }
}

