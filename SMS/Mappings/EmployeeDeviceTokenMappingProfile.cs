using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeDeviceToken entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeDeviceTokenMappingProfile : Profile
    {
        public EmployeeDeviceTokenMappingProfile()
        {
            CreateMap<EmployeeDeviceToken, EmployeeDeviceTokenDto>()
                .ReverseMap();
        }
    }
}
