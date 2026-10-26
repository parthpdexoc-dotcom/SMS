using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping HrEmployeeType entities to DTOs and vice-versa.
    /// </summary>
    public class HrEmployeeTypeMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrEmployeeTypeMappingProfile"/> class.
        /// </summary>
        public HrEmployeeTypeMappingProfile()
        {
            CreateMap<HrEmployeeType, HrEmployeeTypeDto>().ReverseMap();
        }
    }
}

