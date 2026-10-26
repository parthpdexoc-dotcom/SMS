using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Users entities to DTOs and vice-versa.
    /// </summary>
    public class UsersMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersMappingProfile"/> class.
        /// </summary>
        public UsersMappingProfile()
        {
            CreateMap<Users, UsersDto>().ReverseMap();
        }
    }
}

