using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PaymentAllocation entities to DTOs and vice-versa.
    /// </summary>
    public class PaymentAllocationMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAllocationMappingProfile"/> class.
        /// </summary>
        public PaymentAllocationMappingProfile()
        {
            CreateMap<PaymentAllocation, PaymentInformationDto>()
                .ForMember(d => d.ReferenceNumber, o => o.MapFrom(s => s.Payment!.ReferenceNumber))
                .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.Payment!.PaymentDate))
                .ForMember(d => d.PaymentTypeID, o => o.MapFrom(s => s.Payment != null ? s.Payment.PaymentTypeID : null))
                .ForMember(d => d.PaymentTypeName, o => o.MapFrom(s => s.Payment != null && s.Payment.PaymentTypeMaster != null ? s.Payment.PaymentTypeMaster.Name : null))
                .ForMember(d => d.PaymentNumber, o => o.MapFrom(s => s.Payment!.PaymentNumber))
                .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : 0m));


            CreateMap<PaymentAllocation, PaymentInvoiceAllocationDto>()
                .IncludeBase<PaymentAllocation, PaymentInformationDto>()
                .ForMember(d => d.InvoiceAmount, o => o.MapFrom(s => s.Invoice!.Total ?? 0m))
                .ForMember(d => d.InvoiceNo, o => o.MapFrom(s => s.Invoice!.InvoiceNo))
                .ForMember(d => d.InvoiceDate, o => o.MapFrom(s => s.Invoice!.DateCreated))
                .ForMember(d => d.DueDate, o => o.MapFrom(s => s.Invoice!.DueDate))
                .ForMember(d => d.BalanceDue, o => o.MapFrom(s => s.Invoice!.BalanceDue));

            CreateMap<PaymentAllocation, PaymentAllocationDto>().ReverseMap();
        }
    }
}
