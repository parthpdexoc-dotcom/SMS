using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class EmployeePostLocationMappingProfile : Profile
    {
        public EmployeePostLocationMappingProfile()
        {
            CreateMap<EmployeePostLocation, EmployeePostLocationDto>().ReverseMap();
        }
    }
}
