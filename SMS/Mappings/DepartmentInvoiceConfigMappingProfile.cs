using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping DepartmentInvoiceConfig entities to DTOs and vice-versa.
    /// </summary>
    public class DepartmentInvoiceConfigMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentInvoiceConfigMappingProfile"/> class.
        /// </summary>
        public DepartmentInvoiceConfigMappingProfile()
        {
            CreateMap<DepartmentInvoiceConfig, DepartmentInvoiceConfigDto>().ReverseMap();
        }
    }
}
