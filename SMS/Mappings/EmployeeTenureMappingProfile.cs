using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for EmployeeTenure.
    /// </summary>
    public class EmployeeTenureMappingProfile : Profile
    {
        public EmployeeTenureMappingProfile()
        {
            CreateMap<EmployeeTenure, EmployeeTenureDto>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.EmployeeType != null ? src.EmployeeType.Rank : 0))
                .ForMember(dest => dest.EmployeeTypeShort, opt => opt.MapFrom(src => src.EmployeeType != null ? src.EmployeeType.EmployeeTypeShort : null))
                .ReverseMap();
        }
    }
}


