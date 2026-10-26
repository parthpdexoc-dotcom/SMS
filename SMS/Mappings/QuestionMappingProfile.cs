using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Question entities to DTOs and vice-versa.
    /// </summary>
    public class QuestionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionMappingProfile"/> class.
        /// </summary>
        public QuestionMappingProfile()
        {
            CreateMap<Question, QuestionDto>()
                .ReverseMap();
        }
    }
}
