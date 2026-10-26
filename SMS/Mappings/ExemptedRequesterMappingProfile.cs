using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ExemptedRequester entities to DTOs and vice-versa.
    /// </summary>
    public class ExemptedRequesterMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemptedRequesterMappingProfile"/> class.
        /// </summary>
        public ExemptedRequesterMappingProfile()
        {
            CreateMap<ExemptedRequester, ExemptedRequesterDto>().ReverseMap();
        }
    }
}

