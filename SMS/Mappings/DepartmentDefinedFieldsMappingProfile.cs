using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for DepartmentEquipment.
    /// </summary>
    public class DepartmentDefinedFieldsMappingProfile : Profile
    {
        public DepartmentDefinedFieldsMappingProfile()
        {
            CreateMap<DepartmentDefinedFields, DepartmentDefinedFieldsDto>().ReverseMap();
        }
    }
}


