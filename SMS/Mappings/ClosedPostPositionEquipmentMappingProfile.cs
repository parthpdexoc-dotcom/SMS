using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ClosedPostPositionEquipment entities to DTOs and vice-versa.
    /// </summary>
    public class ClosedPostPositionEquipmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionEquipmentMappingProfile"/> class.
        /// </summary>
        public ClosedPostPositionEquipmentMappingProfile()
        {
            CreateMap<ClosedPostPositionEquipment, ClosedPostPositionEquipmentDto>().ReverseMap();

            // Close/split sends SplitPostPositionEquipmentDto; ensure all members map like the base DTO.
            CreateMap<SplitPostPositionEquipmentDto, ClosedPostPositionEquipment>()
                .IncludeBase<ClosedPostPositionEquipmentDto, ClosedPostPositionEquipment>();
        }
    }
}
