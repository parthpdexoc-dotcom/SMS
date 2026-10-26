using SMS.Generic.Common;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Centralized employee validations that span other domains.
    /// </summary>
    public class EmployeeValidationService : IEmployeeValidationService
    {
        private readonly IPostPositionRepository _postPositionRepository;

        public EmployeeValidationService(IPostPositionRepository postPositionRepository)
        {
            _postPositionRepository = postPositionRepository;
        }

        public async Task ValidateCanDeactivateEmployeeAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                return;

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);
            var activeStatuses = new[]
            {
                PostPositionStatus.Assigned,
                PostPositionStatus.SplitDecisionPending,
                PostPositionStatus.Unclosed,
                PostPositionStatus.CancellationPending
            };

            var conflicting = await _postPositionRepository.GetAllAsync(
                filter: x => x.EmployeeID == employeeId &&
                             x.DateOfWork.HasValue &&
                             activeStatuses.Contains(x.PostPositionStatus),
                includes: null,
                pageParam: null).ConfigureAwait(false);

            var hasOngoingPost = conflicting.Any(x =>
                x.DateOfWork!.Value < today ||
                (x.DateOfWork.Value == today &&
                 x.StartTime.HasValue &&
                 x.StartTime.Value <= currentTime));

            if (hasOngoingPost)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "Employee cannot be marked inactive because they are assigned to an ongoing post.");
            }
        }
    }
}
