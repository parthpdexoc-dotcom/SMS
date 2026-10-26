using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Server-side filter for administration requester workspace list (non-daily).
    /// </summary>
    public sealed class RequesterWorkspaceListQuery
    {
        public Guid DepartmentId { get; init; }

        public string? RequesterName { get; init; }

        public DateOnly? StartDate { get; init; }

        public DateOnly? EndDate { get; init; }

        public RequestStatus[]? RequestStatuses { get; init; }

        public RequestTypes[]? RequestTypes { get; init; }

        public bool IncludePast { get; init; }
    }
}
