using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequestActivityType entities to DTOs and vice-versa.
    /// </summary>
    public class RequestActivityTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestActivityTypeMappingProfile"/> class.
        /// </summary>
        public RequestActivityTypeMappingProfile()
        {
            CreateMap<RequestActivityType, RequestActivityTypeDto>().ReverseMap();
        }
    }
}

