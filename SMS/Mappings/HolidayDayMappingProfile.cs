using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping HolidayDay entities to DTOs and vice-versa.
    /// </summary>
    public class HolidayDayMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayDayMappingProfile"/> class.
        /// </summary>
        public HolidayDayMappingProfile()
        {
            CreateMap<HolidayDay, HolidayDayDto>()
                .ReverseMap();
        }
    }
}
