using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for DepartmentEquipment.
    /// </summary>
    public class DepartmentEquipmentMappingProfile : Profile
    {
        public DepartmentEquipmentMappingProfile()
        {
            CreateMap<DepartmentEquipment, DepartmentEquipmentDto>().ReverseMap();
            CreateMap<DepartmentEquipment, DepartmentEquipmentResponseDto>()
                .ForMember(
                    d => d.EquipmentType,
                    o => o.MapFrom(s => s.EquipmentType != null
                        ? s.EquipmentType.EquipementTypeShort
                        : null)
                );
            CreateMap<DepartmentEquipmentDto, DepartmentEquipmentResponseDto>()
                .ForMember(
                    d => d.EquipmentType,
                    o => o.MapFrom(s => s.EquipmentType != null
                        ? s.EquipmentType.EquipementTypeShort
                        : null)
                );
        }
    }
}


