using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostNotes entities to DTOs and vice-versa.
    /// </summary>
    public class PostNotesMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostNotesMappingProfile"/> class.
        /// </summary>
        public PostNotesMappingProfile()
        {
            CreateMap<PostNotes, PostNotesDto>().ReverseMap();
        }
    }
}

