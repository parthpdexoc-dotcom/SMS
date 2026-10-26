using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterManager entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterManagerMappingProfile : Profile
    {
        public RequesterManagerMappingProfile()
        {
            CreateMap<RequesterManager, RequesterManagerDto>().ReverseMap();
            // Detail DTO (with EmployeeName)
            CreateMap<RequesterManager, RequestManagerResponceDto>()
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee.FirstName));
            CreateMap<RequesterManagerDto, RequestManagerResponceDto>()
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee.FirstName));
        }
    }
}


