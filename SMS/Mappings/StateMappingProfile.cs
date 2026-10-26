using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping State entities to DTOs and vice-versa.
    /// </summary>
    public class StateMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMappingProfile"/> class.
        /// </summary>
        public StateMappingProfile()
        {
            CreateMap<State, StateDto>().ReverseMap();
        }
    }
}

