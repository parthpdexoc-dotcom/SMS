using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ExportRoster entities to DTOs and vice-versa.
    /// </summary>
    public class ExportRosterMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRosterMappingProfile"/> class.
        /// </summary>
        public ExportRosterMappingProfile()
        {
            CreateMap<ExportRoster, ExportRosterDto>().ReverseMap();
        }
    }
}

