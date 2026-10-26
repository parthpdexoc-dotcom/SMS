using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentActivityType entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentActivityTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentActivityTypeMappingProfile"/> class.
        /// </summary>
        public DepartmentActivityTypeMappingProfile()
        {
            CreateMap<DepartmentActivityType, DepartmentActivityTypeDto>().ReverseMap();
        }
    }
}
