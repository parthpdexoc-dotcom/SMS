using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmailProcess entities to DTOs and vice-versa.
    /// </summary>
    public class EmailProcessMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailProcessMappingProfile"/> class.
        /// </summary>
        public EmailProcessMappingProfile()
        {
            CreateMap<EmailProcess, EmailProcessDto>().ReverseMap();
        }
    }
}

