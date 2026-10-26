using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequestNotes entities to DTOs and vice-versa.
    /// </summary>
    public class RequestNotesMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestNotesMappingProfile"/> class.
        /// </summary>
        public RequestNotesMappingProfile()
        {
            CreateMap<RequestNotes, RequestNotesDto>().ReverseMap();
        }
    }
}

