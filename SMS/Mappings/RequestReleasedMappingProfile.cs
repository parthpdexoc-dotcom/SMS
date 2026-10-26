using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequestSchedule entities to DTOs and vice-versa.
    /// </summary>
    public class RequestScheduleMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScheduleMappingProfile"/> class.
        /// </summary>
        public RequestScheduleMappingProfile()
        {
            CreateMap<RequestSchedule, RequestScheduleDto>()
                .ForMember(dest => dest.GeneratedScheduleCount, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}

