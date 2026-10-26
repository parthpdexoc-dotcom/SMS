using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Estimate entities to DTOs and vice-versa.
    /// </summary>
    public class EstimateMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EstimateMappingProfile"/> class.
        /// </summary>
        public EstimateMappingProfile()
        {
            CreateMap<Estimate, EstimateDto>().ReverseMap();
        }
    }
}

