using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequestEquipment entities to DTOs and vice-versa.
    /// </summary>
    public class RequestEquipmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEquipmentMappingProfile"/> class.
        /// </summary>
        public RequestEquipmentMappingProfile()
        {
            CreateMap<RequestEquipment, RequestEquipmentDto>().ReverseMap();
            CreateMap<RequestEquipmentDto, RequestEquipmentWithNameDto>();
        }
    }
}
