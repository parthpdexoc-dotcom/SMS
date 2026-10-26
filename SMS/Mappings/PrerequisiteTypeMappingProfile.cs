using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PrerequisiteType entities to DTOs and vice-versa.
    /// </summary>
    public class PrerequisiteTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteTypeMappingProfile"/> class.
        /// </summary>
        public PrerequisiteTypeMappingProfile()
        {
            CreateMap<PrerequisiteType, PrerequisiteTypeDto>().ReverseMap();
        }
    }
}

