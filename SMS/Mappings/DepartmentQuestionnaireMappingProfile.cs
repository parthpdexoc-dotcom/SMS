using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentQuestionnaire entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentQuestionnaireMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentQuestionnaireMappingProfile"/> class.
        /// </summary>
        public DepartmentQuestionnaireMappingProfile()
        {
            CreateMap<DepartmentQuestionnaire, DepartmentQuestionnaireDto>().ReverseMap();
        }
    }
}
