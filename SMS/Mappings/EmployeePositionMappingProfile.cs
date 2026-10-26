using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeePosition entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeePositionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePositionMappingProfile"/> class.
        /// </summary>
        public EmployeePositionMappingProfile()
        {
            CreateMap<EmployeePosition, EmployeePositionDto>().ReverseMap();
        }
    }
}

