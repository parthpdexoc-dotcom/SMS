using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping CancelledPostPosition entities to DTOs and vice-versa.
    /// </summary>
    public class CancelledPostPositionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledPostPositionMappingProfile"/> class.
        /// </summary>
        public CancelledPostPositionMappingProfile()
        {
            CreateMap<CancelledPostPosition, CancelledPostPositionDto>().ReverseMap();
        }
    }
}
