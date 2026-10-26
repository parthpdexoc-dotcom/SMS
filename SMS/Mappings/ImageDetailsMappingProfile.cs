using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ImageDetails entities to DTOs and vice-versa.
    /// </summary>
    public class ImageDetailsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDetailsMappingProfile"/> class.
        /// </summary>
        public ImageDetailsMappingProfile()
        {
            CreateMap<ImageDetails, ImageDetailsDto>().ReverseMap();
        }
    }
}

