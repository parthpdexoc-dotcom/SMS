using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmergencyContact entities to DTOs and vice-versa.
    /// </summary>
    public class EmergencyContactMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyContactMappingProfile"/> class.
        /// </summary>
        public EmergencyContactMappingProfile()
        {
            CreateMap<EmergencyContact, EmergencyContactDto>().ReverseMap();
        }
    }
}

