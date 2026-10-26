using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DivisionAdminOnDuty entities to DTOs and vice-versa.
    /// </summary>
    public class DivisionAdminOnDutyMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionAdminOnDutyMappingProfile"/> class.
        /// </summary>
        public DivisionAdminOnDutyMappingProfile()
        {
            CreateMap<DivisionAdminOnDuty, DivisionAdminOnDutyDto>().ReverseMap();
        }
    }
}

