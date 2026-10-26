using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostEquipment entities to DTOs and vice-versa.
    /// </summary>
    public class PostEquipmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostEquipmentMappingProfile"/> class.
        /// </summary>
        public PostEquipmentMappingProfile()
        {
            CreateMap<PostEquipment, PostEquipmentDto>().ReverseMap();
            CreateMap<PostEquipmentDto, PostEquipmentWithNameDto>();
        }
    }
}
