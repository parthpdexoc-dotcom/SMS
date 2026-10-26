using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Invoice entities to DTOs and vice-versa.
    /// </summary>
    public class InvoiceMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceMappingProfile"/> class.
        /// </summary>
        public InvoiceMappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.RequestName, opt => opt.MapFrom(src => src.Request != null ? src.Request.Title : null))
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.Requester != null ? src.Requester.Name : null))
                .ReverseMap()
                .ForMember(dest => dest.Request, opt => opt.Ignore())
                .ForMember(dest => dest.Requester, opt => opt.Ignore());
        }
    }
}

