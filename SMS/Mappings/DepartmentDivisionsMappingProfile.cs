using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentDivisions entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentDivisionsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsMappingProfile"/> class.
        /// </summary>
        public DepartmentDivisionsMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<DepartmentDivisions, DepartmentDivisionsDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // Service will ensure Guid is set before mapping, so this handles the conversion
            CreateMap<DepartmentDivisionsDto, DepartmentDivisions>()
                .ForMember(dest => dest.DivisionID, opt => opt.MapFrom(src => 
                    src.DivisionID ?? Guid.Empty));
        }
    }
}

