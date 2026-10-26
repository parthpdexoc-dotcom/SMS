using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentConfiguration entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentConfigurationMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentConfigurationMappingProfile"/> class.
        /// </summary>
        public DepartmentConfigurationMappingProfile()
        {
            CreateMap<DepartmentConfiguration, DepartmentConfigurationDto>().ReverseMap();
        }
    }
}

