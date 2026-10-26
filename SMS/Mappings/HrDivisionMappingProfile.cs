using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping HrDivision entities to DTOs and vice-versa.
    /// </summary>
    public class HrDivisionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrDivisionMappingProfile"/> class.
        /// </summary>
        public HrDivisionMappingProfile()
        {
            CreateMap<HrDivision, HrDivisionDto>().ReverseMap();
        }
    }
}

