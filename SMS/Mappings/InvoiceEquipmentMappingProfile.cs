using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping InvoiceEquipment entities to DTOs and vice-versa.
    /// </summary>
    public class InvoiceEquipmentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentMappingProfile"/> class.
        /// </summary>
        public InvoiceEquipmentMappingProfile()
        {
            CreateMap<InvoiceEquipment, InvoiceEquipmentDto>().ReverseMap();
        }
    }
}

