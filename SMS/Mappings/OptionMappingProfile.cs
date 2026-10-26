using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Option entities to DTOs and vice-versa.
    /// </summary>
    public class OptionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMappingProfile"/> class.
        /// </summary>
        public OptionMappingProfile()
        {
            // Map from Entity to DTO (non-nullable Guid to nullable Guid)
            CreateMap<Option, OptionDto>();

            // Map from DTO to Entity (nullable Guid to non-nullable Guid)
            // FluentValidation ensures values are not null before mapping
            CreateMap<OptionDto, Option>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId ?? Guid.Empty));
        }
    }
}
