using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Login entities to DTOs and vice-versa.
    /// </summary>
    public class LoginMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginMappingProfile"/> class.
        /// </summary>
        public LoginMappingProfile()
        {
            CreateMap<Login, LoginRecordDto>().ReverseMap();
        }
    }
}

