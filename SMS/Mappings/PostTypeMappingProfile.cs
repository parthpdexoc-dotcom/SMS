using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostType entities to DTOs and vice-versa.
    /// </summary>
    public class PostTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostTypeMappingProfile"/> class.
        /// </summary>
        public PostTypeMappingProfile()
        {
            CreateMap<PostType, PostTypeDto>().ReverseMap();
        }
    }
}

