using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentResponseArea entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentResponseAreaMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentResponseAreaMappingProfile"/> class.
        /// </summary>
        public DepartmentResponseAreaMappingProfile()
        {
            CreateMap<DepartmentResponseArea, DepartmentResponseAreaDto>().ReverseMap();
        }
    }
}

