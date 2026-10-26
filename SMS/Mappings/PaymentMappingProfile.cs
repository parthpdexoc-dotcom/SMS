using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.PaymentTypeName, o => o.MapFrom(s => s.PaymentTypeMaster != null ? s.PaymentTypeMaster.Name : null))
                .ReverseMap()
                .ForMember(d => d.PaymentTypeMaster, o => o.Ignore());
        }
    }
}
