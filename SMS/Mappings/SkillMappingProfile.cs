using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Skill entities to DTOs and vice-versa.
    /// </summary>
    public class SkillMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillMappingProfile"/> class.
        /// </summary>
        public SkillMappingProfile()
        {
            CreateMap<Skill, SkillDto>().ReverseMap();
        }
    }
}
