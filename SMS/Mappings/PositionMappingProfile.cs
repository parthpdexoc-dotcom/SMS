using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Position entities to DTOs and vice-versa.
    /// </summary>
    public class PositionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionMappingProfile"/> class.
        /// </summary>
        public PositionMappingProfile()
        {
            CreateMap<Position, PositionDto>().ReverseMap();
        }
    }
}

