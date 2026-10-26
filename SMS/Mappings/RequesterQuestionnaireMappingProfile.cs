using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostQuestionnaire entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterQuestionnaireMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterQuestionnaireMappingProfile"/> class.
        /// </summary>
        public RequesterQuestionnaireMappingProfile()
        {
            CreateMap<RequesterQuestionnaire, RequesterQuestionnaireDto>().ReverseMap();
        }
    }
}
