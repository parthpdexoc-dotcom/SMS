using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeAdmin entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeAdminMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeAdminMappingProfile"/> class.
        /// </summary>
        public EmployeeAdminMappingProfile()
        {
            CreateMap<EmployeeAdmin, EmployeeAdminDto>().ReverseMap();
        }
    }
}
