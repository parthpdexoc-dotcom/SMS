using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeSkill entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeSkillMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSkillMappingProfile"/> class.
        /// </summary>
        public EmployeeSkillMappingProfile()
        {
            CreateMap<EmployeeSkill, EmployeeSkillDto>()
                .ForMember(dest => dest.SkillShort, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.SkillShort : null))
                .ReverseMap();
        }
    }
}

