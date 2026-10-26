using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class RequesterLedgerMappingProfile : Profile
    {
        public RequesterLedgerMappingProfile()
        {
            CreateMap<RequesterLedger, RequesterLedgerDto>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.Requester != null ? src.Requester.Name : null));

            CreateMap<RequesterLedgerDto, RequesterLedger>()
                .ForMember(dest => dest.Requester, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByEmployee, opt => opt.Ignore())
                .ForMember(dest => dest.EditedByEmployee, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedByEmployee, opt => opt.Ignore());
        }
    }
}
