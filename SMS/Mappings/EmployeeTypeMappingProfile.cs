using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeType entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeMappingProfile"/> class.
        /// </summary>
        public EmployeeTypeMappingProfile()
        {
            CreateMap<EmployeeType, EmployeeTypeDto>().ReverseMap();
        }
    }
}

