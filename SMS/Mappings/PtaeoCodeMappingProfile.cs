using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PtaeoCode entities to DTOs and Excel rows to PtaeoCodeDto.
    /// </summary>
    public class PtaeoCodeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PtaeoCodeMappingProfile"/> class.
        /// </summary>
        public PtaeoCodeMappingProfile()
        {
            CreateMap<PtaeoCode, PtaeoCodeDto>();
            CreateMap<PtaeoCodeDto, PtaeoCode>()
                .ForMember(dest => dest.PtaeoCodeID, opt => opt.MapFrom(src => src.PtaeoCodeID ?? Guid.Empty));

            CreateMap<Dictionary<string, string>, PtaeoCodeDto>()
                .ConvertUsing<ExcelRowToPtaeoCodeDtoConverter>();
        }
    }
}
