using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentFee entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentFeeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentFeeMappingProfile"/> class.
        /// </summary>
        public DepartmentFeeMappingProfile()
        {
            // Map from Entity to DTO (int to enum)
            CreateMap<DepartmentFee, DepartmentFeeDto>()
                .ForMember(dest => dest.FeeType, opt => opt.MapFrom(src => (FeeType)src.FeeType))
                .ForMember(dest => dest.ReimburseTo, opt => opt.MapFrom(src => (ReimburseTo)src.ReimburseTo));

            // Map from DTO to Entity (enum to int)
            CreateMap<DepartmentFeeDto, DepartmentFee>()
                .ForMember(dest => dest.FeeType, opt => opt.MapFrom(src => (int)src.FeeType))
                .ForMember(dest => dest.ReimburseTo, opt => opt.MapFrom(src => (int)src.ReimburseTo))
                .ForMember(dest => dest.FeeId, opt => opt.MapFrom(src => 
                    src.FeeId ?? Guid.Empty));
        }
    }
}
