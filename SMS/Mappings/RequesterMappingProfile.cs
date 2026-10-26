using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Requesters entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterMappingProfile"/> class.
        /// </summary>
        public RequesterMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<Requester, RequesterDto>().ReverseMap();

            CreateMap<Requester, RequesterResponseDto>()
                .IncludeBase<Requester, RequesterDto>()
                .ForMember(
                    d => d.BusinessType,
                    o => o.MapFrom(s => s.BusinessType != null
                        ? s.BusinessType.BusinessTypeShort
                        : null)
                );

            // When source is RequesterDto (e.g. from service GetAllAsync). BusinessType is left null unless populated elsewhere.
            CreateMap<RequesterDto, RequesterResponseDto>();
        }
    }
}

