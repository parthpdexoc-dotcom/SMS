using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterAuthorized entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterAuthorizedMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterAuthorizedMappingProfile"/> class.
        /// </summary>
        public RequesterAuthorizedMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<RequesterAuthorized, RequesterAuthorizedDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // Service will ensure Guid is set before mapping, so this handles the conversion
            CreateMap<RequesterAuthorizedDto, RequesterAuthorized>()
                .ForMember(dest => dest.RequesterAuthorizedID, opt => opt.MapFrom(src => 
                    src.RequesterAuthorizedID ?? Guid.Empty));
        }
    }
}


