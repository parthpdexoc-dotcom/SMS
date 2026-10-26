using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Admin entities to DTOs and vice-versa.
    /// </summary>
    public class AdminMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminMappingProfile"/> class.
        /// </summary>
        public AdminMappingProfile()
        {
            CreateMap<Admin, AdminDto>().ReverseMap();
        }
    }
}

