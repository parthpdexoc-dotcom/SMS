using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Equipment entities to DTOs and vice-versa.
    /// </summary>
    public class EquipmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentMappingProfile"/> class.
        /// </summary>
        public EquipmentMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<Equipment, EquipmentDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // Service will ensure Guid is set before mapping, so this handles the conversion
            CreateMap<EquipmentDto, Equipment>()
                .ForMember(dest => dest.EquipementID, opt => opt.MapFrom(src => 
                    src.EquipementID ?? Guid.Empty));
        }
    }
}

