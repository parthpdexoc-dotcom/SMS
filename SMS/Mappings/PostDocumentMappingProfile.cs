using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
 

    public class PostDocumentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostDocumentMappingProfile"/> class.
        /// </summary>
        public PostDocumentMappingProfile()
        {
            CreateMap<PostDocument, PostDocumentDto>().ReverseMap();
        }

    }
}
