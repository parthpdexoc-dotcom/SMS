using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Mappings
{
    /// <summary>
    /// Maps legacy requester list DTOs to unified workspace list shapes.
    /// </summary>
    public class RequesterWorkspaceMappingProfile : Profile
    {
        public RequesterWorkspaceMappingProfile()
        {
            CreateMap<RequesterWorkspaceListFilterDto, RequesterRequestSummaryFilterDto>();

            CreateMap<RequesterRequestSummaryDto, RequesterWorkspaceListItemDto>()
                .ForMember(dest => dest.HasUnpaidInvoice, opt => opt.MapFrom(_ => false))
                .AfterMap((src, dest) =>
                {
                    if (dest.Requests == null)
                        return;
                    foreach (var request in dest.Requests)
                        request.RequesterId = src.RequesterId;
                });

            CreateMap<RequesterWithRequestsListDto, RequesterWorkspaceListItemDto>()
                .ForMember(dest => dest.RequestCount, opt => opt.MapFrom(src => src.RequestsCount))
                .ForMember(dest => dest.ProjectNumbers, opt => opt.MapFrom(src =>
                    src.Requests
                        .Where(r => !string.IsNullOrWhiteSpace(r.ProjectNumber))
                        .Select(r => r.ProjectNumber!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(pn => pn, StringComparer.OrdinalIgnoreCase)
                        .ToList()))
                .ForMember(dest => dest.Requests, opt => opt.MapFrom(src => src.Requests));

            CreateMap<RequestScheduleDto, RequesterWorkspaceRequestScheduleDto>()
                .ForMember(dest => dest.ReleaseStatus, opt => opt.MapFrom(src => src.RequestStatus.ToString()));

            CreateMap<RequestScheduleInRequesterListDto, RequesterWorkspaceRequestScheduleDto>()
                .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => ParseRequestStatus(src.ReleaseStatus)))
                .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src => ParseReleaseAs(src.ReleaseAs)));

            CreateMap<RequestDto, RequesterWorkspaceRequestItemDto>()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestID))
                .ForMember(dest => dest.RequesterId, opt => opt.MapFrom(src => src.RequesterID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.RequestSchedules, opt => opt.MapFrom(src => src.RequestSchedules ?? new List<RequestScheduleDto>()))
                .ForMember(dest => dest.RequestSchedule, opt => opt.MapFrom(src =>
                    (src.RequestSchedules ?? new List<RequestScheduleDto>())
                        .OrderBy(s => s.StartDate ?? DateOnly.MaxValue)
                        .ThenBy(s => s.RequestScheduleID)
                        .FirstOrDefault()))
                .ForMember(dest => dest.ReleaseStatus, opt => opt.MapFrom(src =>
                    (src.RequestSchedules ?? new List<RequestScheduleDto>())
                        .OrderBy(s => s.StartDate ?? DateOnly.MaxValue)
                        .ThenBy(s => s.RequestScheduleID)
                        .Select(s => s.RequestStatus.ToString())
                        .FirstOrDefault() ?? string.Empty))
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => (int)src.PostCount));

            CreateMap<RequestInRequesterListDto, RequesterWorkspaceRequestItemDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RequestSchedules, opt => opt.MapFrom(src => src.RequestSchedules))
                .ForMember(dest => dest.RequestSchedule, opt => opt.MapFrom(src =>
                    src.RequestSchedules.OrderBy(s => s.StartDate ?? DateOnly.MaxValue).ThenBy(s => s.RequestScheduleID).FirstOrDefault()))
                .ForMember(dest => dest.RequesterId, opt => opt.Ignore());
        }

        private static RequestStatus ParseRequestStatus(string? value) =>
            Enum.TryParse<RequestStatus>(value, true, out var status) ? status : default;

        private static ReleaseAs ParseReleaseAs(string? value) =>
            Enum.TryParse<ReleaseAs>(value, true, out var release) ? release : default;
    }
}
