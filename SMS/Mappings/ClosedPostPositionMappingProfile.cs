using AutoMapper;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ClosedPostPosition entities to DTOs and vice-versa.
    /// </summary>
    public class ClosedPostPositionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionMappingProfile"/> class.
        /// </summary>
        public ClosedPostPositionMappingProfile()
        {
            CreateMap<ClosedPostPosition, ClosedPostPositionDto>()
                    .ForMember(dest => dest.PostTitle,
                               opt => opt.MapFrom(src => src.Post != null
                                   ? src.Post.PostTitle
                                   : (src.PostPosition != null && src.PostPosition.Post != null
                                       ? src.PostPosition.Post.PostTitle
                                       : null)))
                    .ForMember(dest => dest.DateOfWork,
                               opt => opt.MapFrom(src => src.RequestedStartDate))
                    .ForMember(dest => dest.StartTime,
                               opt => opt.MapFrom(src => src.RequestedStartTime))
                    .ForMember(dest => dest.EndTime,
                               opt => opt.MapFrom(src => src.RequestedEndTime))
                    .ReverseMap()
                    .ForMember(dest => dest.RequestedStartDate,
                               opt => opt.MapFrom(src => src.DateOfWork))
                    .ForMember(dest => dest.RequestedStartTime,
                               opt => opt.MapFrom(src => src.StartTime))
                    .ForMember(dest => dest.RequestedEndTime,
                               opt => opt.MapFrom(src => src.EndTime));

            CreateMap<ClosedPostPosition, CloseAssignmentClosedPostPositionResponseDto>()
                .ForMember(
                    d => d.PostTitle,
                    o => o.MapFrom(s => s.Post != null
                        ? s.Post.PostTitle
                        : (s.PostPosition != null && s.PostPosition.Post != null
                            ? s.PostPosition.Post.PostTitle
                            : null)))
                .ForMember(
                    d => d.RequesterID,
                    o => o.MapFrom(s =>
                        s.Post != null && s.Post.Request != null
                            ? (Guid?)s.Post.Request.RequesterID
                            : (s.PostPosition != null && s.PostPosition.Post != null && s.PostPosition.Post.Request != null
                                ? (Guid?)s.PostPosition.Post.Request.RequesterID
                                : null)))
                .ForMember(
                    d => d.RequesterTitle,
                    o => o.MapFrom(s =>
                        s.Post != null && s.Post.Request != null && s.Post.Request.Requester != null
                            ? s.Post.Request.Requester.Name
                            : (s.PostPosition != null && s.PostPosition.Post != null
                                && s.PostPosition.Post.Request != null && s.PostPosition.Post.Request.Requester != null
                                ? s.PostPosition.Post.Request.Requester.Name
                                : null)))
                .ForMember(
                    d => d.PostLength,
                    o => o.MapFrom(s => s.Post != null
                        ? s.Post.PostLength
                        : (s.PostPosition != null && s.PostPosition.Post != null
                            ? s.PostPosition.Post.PostLength
                            : 0)))
                .ForMember(
                    d => d.EmployeeID,
                    o => o.MapFrom(s => s.PostPosition != null ? s.PostPosition.EmployeeID : (Guid?)null))
                .ForMember(
                    d => d.EnableInvoiceAndPayroll,
                    o => o.MapFrom(s => s.PostPosition != null && s.PostPosition.EnableInvoiceAndPayroll))
                .ForMember(
                    d => d.DateOfWork,
                    o => o.MapFrom(s => s.PostPosition != null ? s.PostPosition.DateOfWork : null))
                .ForMember(
                    d => d.WasUnclosed,
                    o => o.MapFrom(s => s.PostPosition != null
                        ? (s.PostPosition.PostPositionStatus == PostPositionStatus.Unclosed)
                        : (s.Status == ClosedPostPositionStatus.Unclosed)))
                .AfterMap((src, dest) =>
                {
                    var emp = src.PostPosition?.Employee;
                    if (emp != null)
                    {
                        var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(emp.LastName, emp.FirstName);
                        if (!string.IsNullOrWhiteSpace(fullName))
                            dest.CloseBy = fullName;
                        return;
                    }

                    if (src.PostPosition?.IsPlaceholder == true)
                    {
                        var placeholderDisplayName = PlaceholderDto.FormatDisplayName(src.PostPosition.Placeholder);
                        if (!string.IsNullOrWhiteSpace(placeholderDisplayName))
                            dest.CloseBy = placeholderDisplayName;
                    }
                });
        }
    }
}
