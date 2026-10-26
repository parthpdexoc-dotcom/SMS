using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterEmployee entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterEmployeeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterEmployeeMappingProfile"/> class.
        /// </summary>
        public RequesterEmployeeMappingProfile()
        {
            CreateMap<RequesterEmployee, RequesterEmployeeDto>()
                .ReverseMap();
            CreateMap<RequesterEmployee, RequestEmployeeResponseDto>()
               .ForMember(dest => dest.EmployeeName,
                   opt => opt.MapFrom(src => src.Employee.FirstName));
            CreateMap<RequesterEmployeeDto, RequestEmployeeResponseDto>()
               .ForMember(dest => dest.EmployeeName,
                   opt => opt.MapFrom(src => src.Employee.FirstName));
        }
    }
}
