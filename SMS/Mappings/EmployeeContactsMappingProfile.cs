using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeContacts entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeContactsMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactsMappingProfile"/> class.
        /// </summary>
        public EmployeeContactsMappingProfile()
        {
            CreateMap<EmployeeContacts, EmployeeContactsDto>().ReverseMap();
        }
    }
}

