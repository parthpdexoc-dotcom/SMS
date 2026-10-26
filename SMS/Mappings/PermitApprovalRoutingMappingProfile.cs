using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PermitApprovalRouting entities to DTOs and vice-versa.
    /// </summary>
    public class PermitApprovalRoutingMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermitApprovalRoutingMappingProfile"/> class.
        /// </summary>
        public PermitApprovalRoutingMappingProfile()
        {
            CreateMap<PermitApprovalRouting, PermitApprovalRoutingDto>()
                .ReverseMap();

            CreateMap<PermitApprovalRouting, PermitApprovalRoutingResponseDto>()
                .IncludeBase<PermitApprovalRouting, PermitApprovalRoutingDto>()
                .ForMember(
                    d => d.RoutingGroupName,
                    o => o.MapFrom(s => s.Approver != null ? s.Approver.ApproverShort : null));
        }
    }
}
