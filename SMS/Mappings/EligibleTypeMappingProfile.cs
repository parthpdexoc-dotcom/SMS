using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EligibleType entities to DTOs and vice-versa.
    /// </summary>
    public class EligibleTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EligibleTypeMappingProfile"/> class.
        /// </summary>
        public EligibleTypeMappingProfile()
        {
            CreateMap<EligibleType, EligibleTypeDto>().ReverseMap();
        }
    }
}

