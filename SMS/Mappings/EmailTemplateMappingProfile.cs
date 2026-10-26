using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmailTemplate entities to DTOs and vice-versa.
    /// </summary>
    public class EmailTemplateMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateMappingProfile"/> class.
        /// </summary>
        public EmailTemplateMappingProfile()
        {
            CreateMap<EmailTemplate, EmailTemplateDto>().ReverseMap();
        }
    }
}
