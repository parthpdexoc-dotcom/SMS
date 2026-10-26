using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeContactType entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeContactTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactTypeMappingProfile"/> class.
        /// </summary>
        public EmployeeContactTypeMappingProfile()
        {
            CreateMap<EmployeeContactType, EmployeeContactTypeDto>().ReverseMap();
        }
    }
}

