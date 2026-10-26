using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping HRStatus entities to DTOs and vice-versa.
    /// </summary>
    public class HRStatusMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HRStatusMappingProfile"/> class.
        /// </summary>
        public HRStatusMappingProfile()
        {
            CreateMap<HRStatus, HRStatusDto>().ReverseMap();
        }
    }
}

