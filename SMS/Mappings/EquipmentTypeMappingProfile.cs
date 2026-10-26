using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EquipmentType entities to DTOs and vice-versa.
    /// </summary>
    public class EquipmentTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentTypeMappingProfile"/> class.
        /// </summary>
        public EquipmentTypeMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<EquipmentType, EquipmentTypeDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // Service will ensure Guid is set before mapping, so this handles the conversion
            CreateMap<EquipmentTypeDto, EquipmentType>()
                .ForMember(dest => dest.EquipementTypeID, opt => opt.MapFrom(src => 
                    src.EquipementTypeID ?? Guid.Empty));
        }
    }
}


