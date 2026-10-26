using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping FitStandards entities to DTOs and vice-versa.
    /// </summary>
    public class FitStandardsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FitStandardsMappingProfile"/> class.
        /// </summary>
        public FitStandardsMappingProfile()
        {
            CreateMap<FitStandards, FitStandardsDto>().ReverseMap();
        }
    }
}

