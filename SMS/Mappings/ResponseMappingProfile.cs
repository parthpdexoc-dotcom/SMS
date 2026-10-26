using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Response entities to DTOs and vice-versa.
    /// </summary>
    public class ResponseMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMappingProfile"/> class.
        /// </summary>
        public ResponseMappingProfile()
        {
            CreateMap<Response, ResponseDto>().ReverseMap();
        }
    }
}
