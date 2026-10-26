using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping BlockPostDay entities to DTOs and vice-versa.
    /// </summary>
    public class BlockPostDayMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPostDayMappingProfile"/> class.
        /// </summary>
        public BlockPostDayMappingProfile()
        {
            CreateMap<BlockPostDay, BlockPostDayDto>().ReverseMap();
        }
    }
}

