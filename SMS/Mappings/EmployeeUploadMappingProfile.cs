using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping EmployeeUpload entities to DTOs and vice-versa.
    /// </summary>
    public class EmployeeUploadMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeUploadMappingProfile"/> class.
        /// </summary>
        public EmployeeUploadMappingProfile()
        {
            CreateMap<EmployeeUpload, EmployeeUploadDto>().ReverseMap();
        }
    }
}

