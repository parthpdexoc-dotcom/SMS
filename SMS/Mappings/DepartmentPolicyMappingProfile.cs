using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentPolicy entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentPolicyMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentPolicyMappingProfile"/> class.
        /// </summary>
        public DepartmentPolicyMappingProfile()
        {
            // Map DepartmentConfiguration to DepartmentPolicyDto (needed for IncludeMembers)
            // Since DepartmentPolicyDto inherits from DepartmentConfigurationDto,
            // we can use IncludeBase to reuse the existing mapping
            CreateMap<DepartmentConfiguration, DepartmentPolicyDto>()
                .IncludeBase<DepartmentConfiguration, DepartmentConfigurationDto>();

            // Entity -> DTO: flatten DepartmentPolicy + its DepartmentConfiguration
            CreateMap<DepartmentPolicy, DepartmentPolicyDto>()
                .IncludeMembers(dp => dp.DepartmentConfiguration);

            // DTO -> Entity: map only DepartmentPolicy fields.
            // DepartmentConfiguration is managed via its own service/controller
            // to avoid creating duplicate DepartmentConfiguration rows.
            CreateMap<DepartmentPolicyDto, DepartmentPolicy>()
                .ForMember(dest => dest.DepartmentConfiguration, opt => opt.Ignore());
        }
    }
}
