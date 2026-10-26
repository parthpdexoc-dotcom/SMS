using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentAdmin entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentAdminMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentAdminMappingProfile"/> class.
        /// </summary>
        public DepartmentAdminMappingProfile()
        {
            CreateMap<DepartmentAdmin, DepartmentAdminDto>().ReverseMap();
        }
    }
}
