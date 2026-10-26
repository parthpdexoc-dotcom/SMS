using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Prerequisite entities to DTOs and vice-versa.
    /// </summary>
    public class PrerequisiteMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteMappingProfile"/> class.
        /// </summary>
        public PrerequisiteMappingProfile()
        {
            CreateMap<Prerequisite, PrerequisiteDto>().ReverseMap();
        }
    }
}

