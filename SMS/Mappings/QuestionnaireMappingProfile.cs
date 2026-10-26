using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Questionnaire entities to DTOs and vice-versa.
    /// </summary>
    public class QuestionnaireMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireMappingProfile"/> class.
        /// </summary>
        public QuestionnaireMappingProfile()
        {
            CreateMap<Questionnaire, QuestionnaireDto>().ReverseMap();
        }
    }
}
