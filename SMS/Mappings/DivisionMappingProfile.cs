using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Division entities to DTOs and vice-versa.
    /// </summary>
    public class DivisionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionMappingProfile"/> class.
        /// </summary>
        public DivisionMappingProfile()
        {
            CreateMap<Division, DivisionDto>().ReverseMap();
        }
    }
}



