using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping InvoiceEquipmentList entities to DTOs and vice-versa.
    /// </summary>
    public class InvoiceEquipmentListMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentListMappingProfile"/> class.
        /// </summary>
        public InvoiceEquipmentListMappingProfile()
        {
            CreateMap<InvoiceEquipmentList, InvoiceEquipmentListDto>().ReverseMap();
        }
    }
}

