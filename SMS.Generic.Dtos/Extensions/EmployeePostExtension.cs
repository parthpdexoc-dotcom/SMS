using Microsoft.Extensions.Hosting;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System.Linq;

namespace SMS.Generic.Dtos.Extensions;

public static class EmployeePostExtension
{
    public static EmployeePostListingItemDto ToEmployeePostListingItem(this WaitingPostPositionDto s)
    {
        var post = MapCommonFields(
            s.PostID,
            s.Request?.RequestID,
            s.DateOfWork,
            s.Post?.PostStartTime,
            s.Post?.PostLength,
            s.Post != null ? s.Post.PostEndTime : null,
            s.Post?.PostTitle,
            s.Post?.PostLocation,
            s.Request?.Title,
            s.Request?.Location,
            s.Post?.PayCode?.PayCodeValue,
            s.Post?.JobRank,
            s.Request?.IsSpecialEvent ?? false);
        post.WaitingPostPositionID = s.WaitingPostPositionID;
        post.OrderNumber = s.OrderNumber;
        post.HasAppliedInTenure = s.HasAppliedInTenure;
        post.TotalPositions = s.Request?.GetTotalPostPositionsCount() ?? 0;
        post.PostPositionStatus = s.PostPositionStatus;
        post.ReleaseAs = s.Request?.GetReleaseAsByDate(s.DateOfWork) ?? s.Post?.Request?.GetReleaseAsByDate(s.DateOfWork) ?? default;
        return post;
    }

    public static PagedList<EmployeePostListingItemDto> ToEmployeePostListingPaged(this PagedList<AvailablePostDto> source, ReleaseAs releaseAs)
    {
        var items = source.Data.Select(s =>
        {
            var post = MapCommonFields(
                s.PostID,
                s.RequestID,
                s.DateOfWork,
                s.PostStartTime,
                s.PostLength,
                s.PostEndTime,
                s.PostTitle,
                s.PostLocation,
                s.RequestTitle,
                s.RequestLocation,
                s.PayCodeValue,
                s.JobRank,
                s.IsSpecialEvent);
            post.TotalPositions = s.TotalPositions;
            post.AvailablePositions = s.AvailablePositions;
            post.RequesterID = s.RequesterID;
            post.RequesterTitle = s.RequesterTitle;
            post.ReleaseAs = releaseAs;
            return post;
        }).ToList();

        return new PagedList<EmployeePostListingItemDto>(items, new PageParam(source), source.TotalRecords)
        {
            Error = source.Error,
            HasOutstandingReadyToClosePost = source.HasOutstandingReadyToClosePost
        };
    }
    private static EmployeePostListingItemDto MapCommonFields(
        Guid? postId,
        Guid? requestId,
        DateOnly? startDate,
        TimeOnly? startTime,
        int? postLength,
        TimeOnly? postEndTime,
        string? postTitle,
        string? postLocation,
        string? requestTitle,
        string? requestLocation,
        string? payCodeValue,
        string? jobRank,
        bool isSpecialEvent)
    {
        var post = new EmployeePostListingItemDto
        {
            PostID = postId,
            RequestID = requestId,
            RequestTitle = requestTitle,
            PayCodeValue = payCodeValue,
            JobRank = jobRank,
            StartDate = startDate,
            StartTime = startTime,
            PostLength = postLength,
            Title = isSpecialEvent ? requestTitle : postTitle,
            Location = isSpecialEvent ? requestLocation : postLocation,
            IsSpecialEvent = isSpecialEvent
        };
        post.PostEndTime = postEndTime;
        return post;
    }
    public static PagedList<EmployeePostListingItemDto> ToEmployeePostListingPaged(this PagedList<PostTypeResponseDto> source)
    {
        var items = source.Data.Select(s =>
        {
            var post = MapCommonFields(
                s.PostID,
                s.RequestID,
                s.DateOfWork,
                s.PostStartTime,
                s.PostLength,
                s.PostEndTime,
                s.PostTitle,
                s.PostLocation,
                s.RequestTitle,
                null,
                s.PayCodeValue,
                s.JobRank,
                false);
            post.TotalPositions = s.TotalPostPositions;
            post.ReleaseAs = s.ReleaseAs;
            post.PostPositionID = s.PostPositionID;
            post.PostPositionStatus = s.PostPositionStatus;
            post.CancelledBy = s.CancelledBy;
            post.ParentPostPositionId = s.ParentPostPositionId;
            post.ShowEquipment = s.ShowEquipment;
            post.ShowVehicle = s.ShowVehicle;
            post.IsSplit = s.IsSplit;
            post.SplitPostPositionID = s.SplitPostPositionID;
            post.EmployeeID = s.EmployeeID;
            post.EmployeeName = s.EmployeeName;
            post.RequesterID = s.RequesterID;
            post.RequesterTitle = s.RequesterTitle;
            return post;
        }).ToList();

        return new PagedList<EmployeePostListingItemDto>(items, new PageParam(source), source.TotalRecords)
        {
            Error = source.Error,
            HasOutstandingReadyToClosePost = source.HasOutstandingReadyToClosePost
        };
    }

    public static EmployeePostListingItemDto ToEmployeePostListingItem(this ClosedPostPositionDto dto)
    {
        var post = MapCommonFields(
            dto.PostID,
            dto.Post?.RequestID,
            dto.WorkStartDate,
            dto.WorkStartTime,
            dto.HoursWorked,
            dto.Post?.PostEndTime ?? dto.WorkEndTime ?? dto.EndTime,
            dto.PostTitle,
            dto.Post?.PostLocation,
            dto.Post?.Request?.Title,
            dto.Post?.Request?.Location,
            dto.Post?.PayCode?.PayCodeValue,
            dto.Post?.JobRank,
            false);
        post.PostPositionID = dto.PostPositionID;
        post.WorkStartTime = dto.WorkStartTime;
        post.WorkEndTime = dto.WorkEndTime ?? dto.EndTime;
        return post;
    }
    public static PagedList<EmployeePostListingItemDto> ToEmployeePostListingPaged(this PagedList<PostWorkflowAuditDto> source)
    {
        var items = source.Data.Select(s =>
        {
            var post = MapCommonFields(
                s.PostId,
                s.RequestId,
                DateOnly.FromDateTime(s.RecordedDate),
                TimeOnly.FromDateTime(s.RecordedDate),
                null,
                null,
                s.PostTitle,
                null,
                s.RequestTitle,
                null,
                null,
                null,
                false);
            post.PostPositionID = s.PostPositionId;
            post.FailureReason = s.FailureReason;
            post.PostPositionStatus = s.ActionType;
            return post;
        }).ToList();
        return new PagedList<EmployeePostListingItemDto>(items, new PageParam(source), source.TotalRecords)
        {
            Error = source.Error
        };
    }
}