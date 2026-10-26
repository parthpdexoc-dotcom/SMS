using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostsOnDuty entities to DTOs and vice-versa.
    /// </summary>
    public class PostsOnDutyMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostsOnDutyMappingProfile"/> class.
        /// </summary>
        public PostsOnDutyMappingProfile()
        {
            CreateMap<PostsOnDuty, PostsOnDutyDto>().ReverseMap();
        }
    }
}

