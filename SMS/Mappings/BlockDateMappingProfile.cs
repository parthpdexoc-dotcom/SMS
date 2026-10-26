using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping BlockDate entities to DTOs and vice-versa.
    /// </summary>
    public class BlockDateMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDateMappingProfile"/> class.
        /// </summary>
        public BlockDateMappingProfile()
        {
            CreateMap<BlockDate, BlockDateDto>().ReverseMap();
        }
    }
}
