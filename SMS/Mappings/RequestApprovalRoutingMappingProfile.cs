using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping RequestApprovalRouting entities to DTOs and vice-versa.
    /// </summary>
    public class RequestApprovalRoutingMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestApprovalRoutingMappingProfile"/> class.
        /// </summary>
        public RequestApprovalRoutingMappingProfile()
        {
            CreateMap<RequestApprovalRouting, RequestApprovalRoutingDto>()
                .ReverseMap();

            CreateMap<RequestApprovalRouting, RequestApprovalRoutingResponseDto>()
                .IncludeBase<RequestApprovalRouting, RequestApprovalRoutingDto>()
                .ForMember(
                    d => d.RoutingGroupName,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApproverShort
                        : null)
                )
                .ForMember(
                    d => d.ApprovalType,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApprovalType
                        : 0)
                );

            CreateMap<RequestApprovalRoutingDto, RequestApprovalRoutingResponseDto>()
                .ForMember(
                    d => d.RoutingGroupName,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApproverShort
                        : null)
                )
                .ForMember(
                    d => d.ApprovalType,
                    o => o.MapFrom(s => s.Approver != null
                        ? s.Approver.ApprovalType
                        : 0)
                );
        }
    }
}

