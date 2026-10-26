using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping InvoiceDetail entities to DTOs and vice-versa.
    /// </summary>
    public class InvoiceDetailMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetailMappingProfile"/> class.
        /// </summary>
        public InvoiceDetailMappingProfile()
        {
            CreateMap<InvoiceDetail, InvoiceDetailDto>()
                .ForMember(d => d.ClosedPostPosition, o => o.MapFrom(s => s.ClosePostPosition))
                .ForMember(d => d.IsMinimumDue, o => o.MapFrom(s => s.ClosePostPosition != null && s.ClosePostPosition.IsMinimumDue));

            CreateMap<InvoiceDetailDto, InvoiceDetail>()
                .ForMember(d => d.ClosePostPosition, o => o.Ignore())
                .ForMember(d => d.Invoice, o => o.Ignore());

            CreateMap<InvoiceDetailDto, ViewOriginalInvoiceLineItemDto>()
                .ForMember(d => d.OfficerRate, o => o.Ignore())
                .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.GetTotalAmount()))
                .ForMember(d => d.JobDate, o => o.MapFrom(s =>
                    s.ClosedPostPosition != null ? s.ClosedPostPosition.GetWorkDate() : s.JobDate))
                .ForMember(d => d.JobTime, o => o.MapFrom(s =>
                    s.ClosedPostPosition != null ? s.ClosedPostPosition.GetWorkTime() : s.JobTime))
                .ForMember(d => d.PostStartTime, o => o.MapFrom(s =>
                    s.ClosedPostPosition != null ? s.ClosedPostPosition.GetWorkTime() : s.JobTime))
                .ForMember(d => d.PostLength, o => o.MapFrom(s =>
                    s.ClosedPostPosition != null ? s.ClosedPostPosition.GetPostLength() ?? s.PostLength : s.PostLength));
        }
    }
}
