using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RoutingGroup entities to DTOs and vice-versa.
    /// </summary>
    public class RoutingGroupMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingGroupMappingProfile"/> class.
        /// </summary>
        public RoutingGroupMappingProfile()
        {
            CreateMap<RoutingGroup, RoutingGroupDto>()
                .ForMember(d => d.EmployeeIDs, o => o.MapFrom(s => s.EmployeeIDs))
                .ForMember(d => d.ApprovalType, o => o.MapFrom(s => s.ApprovalType))
                .ReverseMap()
                .ForMember(d => d.EmployeeIDs, o => o.MapFrom(s => s.EmployeeIDs))
                .ForMember(d => d.ApprovalType, o => o.MapFrom(s => s.ApprovalType));

            CreateMap<RoutingGroupDto, RoutingGroupDetailsDto>();
        }
    }
}
