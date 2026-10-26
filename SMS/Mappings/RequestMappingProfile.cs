using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Request entities to DTOs and vice-versa.
    /// </summary>
    public class RequestMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMappingProfile"/> class.
        /// </summary>
        public RequestMappingProfile()
        {
            CreateMap<Request, RequestDto>();
            CreateMap<RequestDto, Request>()
                .ForMember(dest => dest.SpecialRate, opt => opt.MapFrom(src => src.SpecialRate ?? 0m));
            CreateMap<RequestType, RequestTypeDto>().ReverseMap();
            CreateMap<RequestSchedule, RequestScheduleDto>()
                .ForMember(dest => dest.GeneratedScheduleCount, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RequestSchedule, RequestScheduleInRequesterListDto>()
                .ForMember(dest => dest.ReleaseStatus, opt => opt.MapFrom(src => src.RequestStatus.ToString()))
                .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src => src.ReleaseAs.ToString()));

            CreateMap<Request, RequestInRequesterListDto>()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestID))
                .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => src.FiscalYear))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ReleaseStatus, opt => opt.MapFrom(src => GetReleaseStatus(src)))
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts != null ? src.Posts.Count : 0))
                .ForMember(dest => dest.RequestSchedules, opt => opt.MapFrom(src => GetOrderedSchedules(src)))
                .ForMember(dest => dest.Posts, opt => opt.Ignore());

            CreateMap<Post, PostWithEmployeeLocationDto>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostID))
                .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.PostTitle ?? string.Empty))
                .ForMember(dest => dest.PostLocation, opt => opt.MapFrom(src => src.PostLocation ?? string.Empty))
                .ForMember(dest => dest.TotalPositions, opt => opt.MapFrom(src => src.TotalPostPositions))
                .ForMember(dest => dest.PostPositions, opt => opt.Ignore());

            CreateMap<PostPosition, PostPositionEmployeeLocationDto>()
                .ForMember(dest => dest.PostPositionId, opt => opt.MapFrom(src => src.PostPositionID))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PostPositionStatus))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null ? BaseDto.FormatDisplayName(src.Employee.LastName, src.Employee.FirstName) : string.Empty))
                .ForMember(dest => dest.BadgeNumber, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.BadgeNumber : null))
                .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.EmployeeType != null
                        ? (src.Employee.EmployeeType.EmployeeTypeShort ?? src.Employee.EmployeeType.EmployeeTypeLong)
                        : null))
                .ForMember(dest => dest.EmployeePostLocations, opt => opt.Ignore());

            CreateMap<EmployeePostLocation, EmployeePostLocationItemDto>()
                .ForMember(dest => dest.EmployeePostLocationId, opt => opt.MapFrom(src => src.EmployeePostLocationID))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeID));
        }

        private static string GetReleaseStatus(Request request)
        {
            var schedule = GetOrderedSchedules(request).FirstOrDefault();
            return schedule != null ? schedule.RequestStatus.ToString() : string.Empty;
        }

        private static IEnumerable<RequestSchedule> GetOrderedSchedules(Request request) =>
            (request.RequestSchedules ?? Enumerable.Empty<RequestSchedule>())
                .OrderBy(s => s.StartDate ?? DateOnly.MaxValue)
                .ThenBy(s => s.RequestScheduleID);
    }
}
