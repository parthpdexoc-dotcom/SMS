using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Document entities to DTOs and vice-versa.
    /// </summary>
    public class DocumentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMappingProfile"/> class.
        /// </summary>
        public DocumentMappingProfile()
        {
            CreateMap<Document, DocumentDto>().ReverseMap();
        }
    }
}
