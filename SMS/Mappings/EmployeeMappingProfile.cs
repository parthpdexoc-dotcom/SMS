using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Employee entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeMappingProfile"/> class.
        /// </summary>
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();

            CreateMap<Dictionary<string, string>, EmployeeDto>()
                .ConvertUsing<ExcelRowToEmployeeDtoConverter>();

            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(
                    d => d.Status,
                    o => o.MapFrom(s => s.Status != null ? s.Status.StatusShort : null)
                )
                .ForMember(
                    d => d.EmployeeType,
                    o => o.MapFrom(s => s.EmployeeType != null ? s.EmployeeType.EmployeeTypeShort : null)
                );
            // EmployeeDto -> EmployeeResponseDto: source has Status/EmployeeType as DTOs, destination has them as strings (property hiding).
            // Prevent convention from mapping source Status/EmployeeType to dest; only use our MapFrom for the string properties.
            CreateMap<EmployeeDto, EmployeeResponseDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status != null ? s.Status.StatusShort : null))
                .ForMember(d => d.EmployeeType, o => o.MapFrom(s => s.EmployeeType != null ? s.EmployeeType.EmployeeTypeShort : null))
                .ForSourceMember(s => s.Status, o => o.DoNotValidate())
                .ForSourceMember(s => s.EmployeeType, o => o.DoNotValidate());
        }
    }
}



