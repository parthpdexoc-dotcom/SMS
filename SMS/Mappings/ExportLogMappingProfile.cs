using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ExportLog entities to DTOs and vice-versa.
    /// </summary>
    public class ExportLogMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportLogMappingProfile"/> class.
        /// </summary>
        public ExportLogMappingProfile()
        {
            CreateMap<ExportLog, ExportLogDto>().ReverseMap();
        }
    }
}

