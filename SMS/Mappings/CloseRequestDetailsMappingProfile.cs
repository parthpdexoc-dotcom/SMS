using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping CloseRequestDetails entities to DTOs and vice-versa.
    /// </summary>
    public class CloseRequestDetailsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseRequestDetailsMappingProfile"/> class.
        /// </summary>
        public CloseRequestDetailsMappingProfile()
        {
            CreateMap<CloseRequestDetails, CloseRequestDetailsDto>().ReverseMap();
        }
    }
}

