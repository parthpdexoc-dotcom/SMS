using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class RefundPaymentAllocationMappingProfile : Profile
    {
        public RefundPaymentAllocationMappingProfile()
        {
            CreateMap<RefundPaymentAllocation, RefundPaymentAllocationDto>().ReverseMap();
        }
    }
}
