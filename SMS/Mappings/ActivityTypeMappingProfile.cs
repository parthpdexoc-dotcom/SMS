using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ActivityType entities to DTOs and vice-versa.
    /// </summary>
    public class ActivityTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityTypeMappingProfile"/> class.
        /// </summary>
        public ActivityTypeMappingProfile()
        {
            CreateMap<ActivityType, ActivityTypeDto>().ReverseMap();
        }
    }
}

