using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping AdminType entities to DTOs and vice-versa.
    /// </summary>
    public class AdminTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTypeMappingProfile"/> class.
        /// </summary>
        public AdminTypeMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<AdminType, AdminTypeDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // Service will ensure Guid is set before mapping, so this handles the conversion
            CreateMap<AdminTypeDto, AdminType>()
                .ForMember(dest => dest.AdminTypeID, opt => opt.MapFrom(src => 
                    src.AdminTypeID ?? Guid.Empty));
        }
    }
}

