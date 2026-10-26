using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Role entities to DTOs and vice-versa.
    /// </summary>
    public class RoleMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleMappingProfile"/> class.
        /// </summary>
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}

