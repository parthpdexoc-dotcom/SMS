using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmailAccessCode entities to DTOs and vice-versa.
    /// </summary>
    public class EmailAccessCodeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAccessCodeMappingProfile"/> class.
        /// </summary>
        public EmailAccessCodeMappingProfile()
        {
            CreateMap<EmailAccessCode, EmailAccessCodeDto>().ReverseMap();
        }
    }
}

