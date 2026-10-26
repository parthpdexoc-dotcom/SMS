using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping HrRequestClass entities to DTOs and vice-versa.
    /// </summary>
    public class HrRequestClassMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrRequestClassMappingProfile"/> class.
        /// </summary>
        public HrRequestClassMappingProfile()
        {
            CreateMap<HrRequestClass, HrRequestClassDto>().ReverseMap();
        }
    }
}

