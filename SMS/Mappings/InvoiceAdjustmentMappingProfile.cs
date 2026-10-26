using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping InvoiceAdjustment entities to DTOs and vice-versa.
    /// </summary>
    public class InvoiceAdjustmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceAdjustmentMappingProfile"/> class.
        /// </summary>
        public InvoiceAdjustmentMappingProfile()
        {
            CreateMap<InvoiceAdjustment, InvoiceAdjustmentDto>().ReverseMap();
        }
    }
}

