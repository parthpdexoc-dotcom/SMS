using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping VehicleApprovalRouting entities to DTOs and vice-versa.
    /// </summary>
    public class VehicleApprovalRoutingMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApprovalRoutingMappingProfile"/> class.
        /// </summary>
        public VehicleApprovalRoutingMappingProfile()
        {
            CreateMap<VehicleApprovalRouting, VehicleApprovalRoutingDto>()
                .ReverseMap();

            CreateMap<VehicleApprovalRouting, VehicleApprovalRoutingResponseDto>()
                .IncludeBase<VehicleApprovalRouting, VehicleApprovalRoutingDto>()
                .ForMember(
                    d => d.RoutingGroupName,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApproverShort
                        : null)
                );
            CreateMap<VehicleApprovalRoutingDto, VehicleApprovalRoutingResponseDto>()
                .ForMember(
                    d => d.RoutingGroupName,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApproverShort
                        : null)
                );
        }
    }
}

