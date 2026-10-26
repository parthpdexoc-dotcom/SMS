using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping BusinessType entities to DTOs and vice-versa.
    /// </summary>
    public class BusinessTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTypeMappingProfile"/> class.
        /// </summary>
        public BusinessTypeMappingProfile()
        {
            CreateMap<BusinessType, BusinessTypeDto>().ReverseMap();
        }
    }
}

