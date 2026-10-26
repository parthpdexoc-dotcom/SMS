using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Department entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentMappingProfile"/> class.
        /// </summary>
        public DepartmentMappingProfile()
        {
            // Map from Entity to DTO (includes nested DepartmentPolicy and DepartmentConfiguration)
            // AutoMapper will automatically use DepartmentPolicyMappingProfile and DepartmentConfigurationMappingProfile
            // for nested object mapping when they are not null
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.DepartmentPolicy, opt => opt.MapFrom(src => src.DepartmentPolicy))
                .ForMember(dest => dest.DepartmentConfiguration, opt => opt.MapFrom(src => src.DepartmentConfiguration));

            // Map from DTO to Entity (exclude nested objects to avoid circular references and complex update scenarios)
            // Navigation properties should be managed separately through their respective services
            CreateMap<DepartmentDto, Department>()
                .ForMember(dest => dest.DepartmentPolicy, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentConfiguration, opt => opt.Ignore());
        }
    }
}




