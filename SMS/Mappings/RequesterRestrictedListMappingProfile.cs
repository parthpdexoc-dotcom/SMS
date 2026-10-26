using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Linq;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequesterRestrictedList entities to DTOs and vice-versa.
    /// </summary>
    public class RequesterRestrictedListMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterRestrictedListMappingProfile"/> class.
        /// </summary>
        public RequesterRestrictedListMappingProfile()
        {
            CreateMap<RequesterRestrictedList, RequesterRestrictedListDto>()
                .ReverseMap()
                .ForMember(d => d.Requester, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore());

            CreateMap<RequesterRestrictedList, RequesterRestrictedListResponseDto>()
                .ForMember(
                    d => d.RequesterName,
                    o => o.MapFrom(s => s.Requester != null ? s.Requester.Name : null)
                )
                .ForMember(
                    d => d.EmployeeName,
                    o => o.MapFrom(s => s.Employee != null 
                        ? string.Join(" ", new[] { s.Employee.FirstName, s.Employee.LastName }.Where(n => !string.IsNullOrWhiteSpace(n)))
                        : null)
                );
            CreateMap<RequesterRestrictedListDto, RequesterRestrictedListResponseDto>()
                .ForMember(
                    d => d.RequesterName,
                    o => o.MapFrom(s => s.Requester != null ? s.Requester.Name : null)
                )
                .ForMember(
                    d => d.EmployeeName,
                    o => o.MapFrom(s => s.Employee != null
                        ? string.Join(" ", new[] { s.Employee.FirstName, s.Employee.LastName }.Where(n => !string.IsNullOrWhiteSpace(n)))
                        : null)
                );
        }
    }
}

