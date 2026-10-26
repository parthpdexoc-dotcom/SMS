using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PayCode entities to DTOs and vice-versa.
    /// </summary>
    public class PayCodeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayCodeMappingProfile"/> class.
        /// </summary>
        public PayCodeMappingProfile()
        {
            CreateMap<PayCode, PayCodeDto>().ReverseMap();
        }
    }
}

