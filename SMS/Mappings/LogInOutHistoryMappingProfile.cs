using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping LogInOutHistory entities to DTOs and vice-versa.
    /// </summary>
    public class LogInOutHistoryMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOutHistoryMappingProfile"/> class.
        /// </summary>
        public LogInOutHistoryMappingProfile()
        {
            CreateMap<LogInOutHistory, LogInOutHistoryDto>().ReverseMap();
        }
    }
}

