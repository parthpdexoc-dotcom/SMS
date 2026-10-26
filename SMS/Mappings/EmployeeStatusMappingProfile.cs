using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeStatus entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeStatusMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusMappingProfile"/> class.
        /// </summary>
        public EmployeeStatusMappingProfile()
        {
            CreateMap<EmployeeStatus, EmployeeStatusDto>().ReverseMap();
        }
    }
}

