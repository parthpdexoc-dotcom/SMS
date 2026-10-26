using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Mapping;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Repositories.Repositories;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing tenure waiting position operations using the generic service pattern.
    /// </summary>
    public class WaitingPostPositionService : GenericService<WaitingPostPositionDto, WaitingPostPosition>, IWaitingPostPositionService
    {
        private readonly ILogger _logger;
        private readonly IWaitingPostPositionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly IRequestService _requestService;
        private readonly IRequestScheduleService _requestScheduleService;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenParser _tokenParser;
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingPostPositionService"/> class.
        /// </summary>
        /// <param name="repository">The tenure waiting position repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="postService">The post service instance.</param>
        /// <param name="requestService">The request service (validates special-event requests).</param>
        /// <param name="employeeService">The employee service (department checks for overrides).</param>
        /// <param name="tokenParser">The token parser instance.</param>
        public WaitingPostPositionService(
            IWaitingPostPositionRepository repository,
            IMapper mapper,
            ILogger<WaitingPostPositionService> logger,
            IPostService postService,
            IRequestService requestService,
            IEmployeeService employeeService,
            IRequestScheduleService requestScheduleService,
        ITokenParser tokenParser) : base(repository, mapper, "WaitingPostPositionID", tokenParser)
        {
            _logger = logger;
            _postService = postService;
            _requestService = requestService;
            _employeeService = employeeService;
            _requestScheduleService = requestScheduleService;
            _mapper = mapper;
            _repository = repository;
            _tokenParser = tokenParser;
        }

        /// <inheritdoc />
        public override async Task<WaitingPostPositionDto> UpsertAsync(WaitingPostPositionDto dto, bool isSuperUser = false)
        {
            var isNew = !dto.WaitingPostPositionID.HasValue || dto.WaitingPostPositionID == Guid.Empty;

            if (dto.PostPositionStatus == PostPositionStatus.Waiting)
            {
                if (isNew)
                {
                    if (dto.PostID.HasValue && dto.PostID.Value != Guid.Empty)
                        dto.OrderNumber = await ComputeNextWaitingOrderNumberAsync(dto.PostID.Value, dto.DateOfWork).ConfigureAwait(false);
                    else
                        throw new WebApiException(
                            HttpStatusCode.BadRequest,
                            ((int)HttpStatusCode.BadRequest).ToString(),
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_POST_POSITION_POST_ID_REQUIRED));
                }
                else
                {
                    var existing = await GetByIdAsync(dto.WaitingPostPositionID!.Value).ConfigureAwait(false);
                    if (existing != null)
                    {
                        dto.OrderNumber = existing.OrderNumber;
                        if (existing.HasAppliedInTenure)
                            dto.HasAppliedInTenure = true;
                    }
                }
            }
            else if (dto.PostPositionStatus == PostPositionStatus.SpecialEvent)
            {
                if (isNew)
                {
                    if (dto.RequestId.HasValue && dto.RequestId.Value != Guid.Empty)
                        dto.OrderNumber = await ComputeNextSpecialEventOrderNumberByRequestAsync(dto.RequestId.Value, dto.DateOfWork)
                            .ConfigureAwait(false);
                }
                else
                {
                    var existing = await GetByIdAsync(dto.WaitingPostPositionID!.Value).ConfigureAwait(false);
                    if (existing != null)
                    {
                        dto.OrderNumber = existing.OrderNumber;
                        if (existing.HasAppliedInTenure)
                            dto.HasAppliedInTenure = true;
                    }
                }
            }
            if (!dto.HasAppliedInTenure)
            {
                var requestId = dto.RequestId;
                if (requestId.HasValue && requestId.Value != Guid.Empty)
                    dto.HasAppliedInTenure = await _requestScheduleService
                        .GetHasAppliedInTenureByDateAsync(requestId.Value, dto.DateOfWork)
                        .ConfigureAwait(false);
            }
            return await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);
        }

        private async Task<int> ComputeNextWaitingOrderNumberAsync(Guid postId, DateOnly dateOfWork)
        {
            return await ComputeNextOrderNumberAsync(
                    x => x.PostID == postId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == PostPositionStatus.Waiting)
                .ConfigureAwait(false);
        }
        private async Task<int> ComputeNextOrderNumberAsync(Expression<Func<WaitingPostPosition, bool>> filter)
        {
            var cohort = (await _repository
                .GetAllAsync(filter, includes: null, pageParam: null)
                .ConfigureAwait(false)).ToList();

            if (cohort.Count == 0)
                return 1;

            return cohort.Max(x => x.OrderNumber) + 1;
        }

        private async Task<int> ComputeNextSpecialEventOrderNumberByRequestAsync(Guid requestId, DateOnly dateOfWork)
        {
            return await ComputeNextOrderNumberAsync(
                    x => x.RequestID == requestId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == PostPositionStatus.SpecialEvent)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> MarkWaitingRowAssignedAsync(Guid waitingPostPositionId, Guid? assignedPostId = null, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            var entity = await GetByIdAsync(waitingPostPositionId).ConfigureAwait(false);
            if (entity == null || (entity.PostPositionStatus != PostPositionStatus.Waiting && entity.PostPositionStatus != PostPositionStatus.SpecialEvent))
                return false;

            var oldOrder = entity.OrderNumber;
            var postId = entity.PostID;
            var requestId = entity.RequestId;
            var dateOfWork = entity.DateOfWork;
            var sourceStatus = entity.PostPositionStatus;

            await UpdateAsync(
                x => x.WaitingPostPositionID == waitingPostPositionId,
                e =>
                {
                    e.PostPositionStatus = PostPositionStatus.Assigned;
                    if (assignedPostId.HasValue && assignedPostId.Value != Guid.Empty)
                        e.PostID = assignedPostId.Value;
                    e.OrderNumber = 0;
                    e.ProcessingStatus = ProcessingStatus.Complete;
                    e.ProcessedAt = DateTime.UtcNow;
                    e.EditedDate = DateTime.UtcNow;
                }).ConfigureAwait(false);

            await ReorderWaitingQueueAsync(postId,requestId, dateOfWork, sourceStatus, oldOrder);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> MarkWaitingRowDeclinedForJobAsync(Guid waitingPostPositionId, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            var entity = await GetByIdAsync(waitingPostPositionId).ConfigureAwait(false);
            if (entity == null || entity.PostPositionStatus != PostPositionStatus.Waiting)
                return false;

            await UpdateAsync(
                x => x.WaitingPostPositionID == waitingPostPositionId,
                e =>
                {
                    e.PostPositionStatus = PostPositionStatus.Declined;
                    e.OrderNumber = 0;
                    e.EditedDate = DateTime.UtcNow;
                }).ConfigureAwait(false);
            await ReorderWaitingQueueAsync(entity.PostID, entity.RequestId, entity.DateOfWork, entity.PostPositionStatus, entity.OrderNumber);
            return true;
        }
        private async Task ReorderWaitingQueueAsync(Guid? postId,Guid? requestId, DateOnly dateOfWork, PostPositionStatus sourceStatus, int oldOrder)
        {
            if (oldOrder < 1)
                return;

            if (postId.HasValue && postId.Value != Guid.Empty)
            {
                await UpdateAsync(
                    x => x.PostID == postId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == sourceStatus &&
                         x.OrderNumber > oldOrder,
                    e =>
                    {
                        e.OrderNumber = e.OrderNumber - 1;
                        e.EditedDate = DateTime.UtcNow;
                    }).ConfigureAwait(false);
            }
            else if (requestId.HasValue && requestId.Value != Guid.Empty)
            {
                await UpdateAsync(
                    x => x.RequestID == requestId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == sourceStatus &&
                         x.OrderNumber > oldOrder,
                    e =>
                    {
                        e.OrderNumber = e.OrderNumber - 1;
                        e.EditedDate = DateTime.UtcNow;
                    }).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Gets the employee ID from token with validation.
        /// </summary>
        private Guid? GetValidatedEmployeeId()
        {
            var employeeId = _tokenParser.GetEmployeeID();
            if (employeeId == Guid.Empty)
            {
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_ID_NOT_FOUND_IN_TOKEN));
            }
            return employeeId;
        }
        public async Task<List<WaitingPostPositionDto>> GetWaitingPostPositionsByEmployeeAndDateRangeAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, Guid? departmentId = null)
        {
            var waitingPostPosition = await _repository.GetWaitingPostPositionsByEmployeeAndDateRangeAsync(employeeId, startDate, endDate, departmentId);
            return _mapper.Map<List<WaitingPostPositionDto>>(waitingPostPosition);
        }
        /// <inheritdoc />
        public async Task<PagedList<EmployeePostListingItemDto>> GetForLoggedInEmployeeAsync(
            IReadOnlyList<ReleaseAs> releaseAsValues,
            bool isViewWLHistory = false,
            PageParam? pagination = null)
        {
            if (releaseAsValues == null || releaseAsValues.Count == 0)
                throw new ArgumentException("At least one release-as value is required.", nameof(releaseAsValues));

            var releaseAsSet = releaseAsValues.Distinct().ToHashSet();

            var employeeId = GetValidatedEmployeeId();
            var effectivePageParam = pagination ?? new PageParam();
            var employeeIdValue = employeeId!.Value;
            var departmentNow = GetCurrentDepartmentDateTime();

            // Eager-load Post.Request: tenure (Waiting) rows often have RequestID null; AutoMapper fills DTO.Request from Post.Request (see WaitingPostPositionMappingProfile). GenericRepository only uses Include(), so Post.Request was never loaded.
            var entities = await _repository.GetForLoggedInEmployeeListingAsync(employeeIdValue).ConfigureAwait(false);
            var dtos = _mapper.Map<List<WaitingPostPositionDto>>(entities);
            var listings = dtos
                        .Select(d => d.ToEmployeePostListingItem())
                        .Where(d => releaseAsSet.Contains(d.ReleaseAs))
                        .Where(d => isViewWLHistory || d.IsWithinWaitingListLookbackWindow(departmentNow))
                        .OrderBy(d => d.StartDate ?? DateOnly.MaxValue)
                        .ThenBy(d => d.StartTime ?? TimeOnly.MaxValue)
                        .ToList();
            var totalCount = listings.Count;
            var page = effectivePageParam.Page ?? PageParam.MinPage;
            var pageSize = effectivePageParam.PageSize ?? effectivePageParam.Limit ?? PageParam.MinLimit;
            var skip = Math.Max(0, (page - 1) * pageSize);
            var pageItems = listings.Skip(skip).Take(pageSize).ToList();

            return new PagedList<EmployeePostListingItemDto>(pageItems, effectivePageParam, totalCount);
        }

        public async Task<SuccessResult<WaitingPostPositionDto>> AssignWaitingPostPositiontoMeAsync(Guid postId, DateOnly dateOfWork, Guid? employeeId = null)
        {
            var result = new SuccessResult<WaitingPostPositionDto>();
            var (subjectEmployeeId, _) = await EmployeeJobsSubjectHelper.ResolveSubjectEmployeeAsync(
                employeeId,
                _tokenParser,
                _employeeService).ConfigureAwait(false);
            employeeId = subjectEmployeeId == Guid.Empty ? null : subjectEmployeeId;

            if (!employeeId.HasValue || employeeId == Guid.Empty)
            {
                result.Status = 401;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_EMPLOYEE_ID_MISSING);
                return result;
            }

            try
            {
                var post = await _postService.GetPostWithRequestAsync(postId);
                if (post == null)
                {
                    result.Status = 404;
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_JOB_NOT_FOUND);
                    return result;
                }

                var dto = new WaitingPostPositionDto(post, employeeId.Value, dateOfWork);
                dto.RequestId = post.RequestID;
                var createdDto = await UpsertAsync(dto);
                result.Status = 200;
                result.Message = $"{post.PostTitle}: added to interest list";
                result.Data = createdDto;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add PostId {PostId} to interest list for EmployeeId {EmployeeId} on {DateOfWork}", postId, employeeId, dateOfWork);
                result.Status = 400;
                result.Message = ex.Message ?? "Failed to added in interest list";
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<SuccessResult<WaitingPostPositionDto>> ApplySpecialEventAsync(Guid requestId, DateOnly dateOfWork, Guid? employeeId = null)
        {
            var result = new SuccessResult<WaitingPostPositionDto>();
            var (subjectEmployeeId, _) = await EmployeeJobsSubjectHelper.ResolveSubjectEmployeeAsync(
                employeeId,
                _tokenParser,
                _employeeService).ConfigureAwait(false);
            employeeId = subjectEmployeeId == Guid.Empty ? null : subjectEmployeeId;

            if (!employeeId.HasValue || employeeId == Guid.Empty)
            {
                result.Status = 401;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_ID_MISSING_FROM_TOKEN);
                return result;
            }

            if (requestId == Guid.Empty)
            {
                result.Status = 400;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_CANNOT_BE_EMPTY);
                return result;
            }

            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
            {
                result.Status = 404;
                result.Message = string.Format(
                    CultureInfo.InvariantCulture,
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND),
                    "Request",
                    requestId);
                return result;
            }

            var tokenDepartmentId = _tokenParser.GetDepartmentID();
            if (tokenDepartmentId.HasValue && request.DepartmentID != tokenDepartmentId.Value)
            {
                result.Status = 403;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT);
                return result;
            }

            if (request.IsSpecialEvent != true)
            {
                result.Status = 400;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_POST_POSITION_NOT_SPECIAL_EVENT_REQUEST);
                return result;
            }

            var alreadyApplied = (await _repository
                .GetAllAsync(
                    x => x.RequestID == requestId &&
                         x.DateOfWork == dateOfWork &&
                         x.EmployeeID == employeeId.Value &&
                         x.PostPositionStatus == PostPositionStatus.SpecialEvent,
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false)).Any();

            if (alreadyApplied)
            {
                result.Status = 409;
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_POST_POSITION_SPECIAL_EVENT_ALREADY_APPLIED);
                return result;
            }

            var dto = new WaitingPostPositionDto
            {
                PostID = null,
                RequestId = requestId,
                EmployeeID = employeeId.Value,
                DateOfWork = dateOfWork,
                PostPositionStatus = PostPositionStatus.SpecialEvent,
                OrderNumber = 0,
                ProcessingStatus = ProcessingStatus.Pending
            };

            try
            {
                var createdDto = await UpsertAsync(dto).ConfigureAwait(false);
                result.Status = 200;
                result.Message = ModelErrorCodes.GetErrorMessage(ModelErrorCodes.SPECIALJOBSUCCESSMESSAGE);
                result.Data = createdDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply special event for RequestId {RequestId}, EmployeeId {EmployeeId} on {DateOfWork}", requestId, employeeId, dateOfWork);
                result.Status = 400;
                result.Message = ex.Message;
            }

            return result;
        }
        public async Task<int> UpdateStatusByIdAsync(Guid waitingPostPositionId, PostPositionStatus status)
        {
            var employeeId = GetValidatedEmployeeId();
            var employeeIdValue = employeeId!.Value;

            var existingWaitingPostPosition = await GetByIdAsync(waitingPostPositionId);
            if (existingWaitingPostPosition == null)
            {
                throw new Exception(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_NOT_FOUND_BY_ID, waitingPostPositionId));
            }

            if (existingWaitingPostPosition.EmployeeID != employeeIdValue)
            {
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_UPDATE_UNAUTHORIZED));
            }

            var updatedCount = await UpdateAsync(
                filter: x => x.WaitingPostPositionID == waitingPostPositionId && x.EmployeeID == employeeIdValue,
                updateAction: entity => entity.PostPositionStatus = status
            );

            return updatedCount;
        }

        /// <inheritdoc />
        public async Task<List<WaitingPostPositionDto>> GetForPostsForJobAsync(List<Guid> postIds, CancellationToken cancellationToken = default)
        {
            if (postIds == null || postIds.Count == 0)
                return new List<WaitingPostPositionDto>();

            // Job should only process active waiting rows; cancelled rows stay in table for audit.
            Expression<Func<WaitingPostPosition, bool>> filter =
                x => x.PostID.HasValue &&
                     postIds.Contains(x.PostID.Value) &&
                     x.PostPositionStatus == PostPositionStatus.Waiting;
            Expression<Func<WaitingPostPosition, object>>[] includes =
            {
                x => x.Post,
                x => x.Post.Request,
                x => x.Post.Request.RequestType,
                x => x.Post.Request.RequestSchedules,
                x => x.Employee
            };

            // Use repository directly (no token-scoped filtering)
            var entities = (await _repository.GetAllAsync(filter, includes, pageParam: null).ConfigureAwait(false)).ToList();
            return _mapper.Map<List<WaitingPostPositionDto>>(entities);
        }

        /// <inheritdoc />
        public async Task<List<WaitingPostPositionResponseDto>> GetWaitingListEmployee(
            Guid postId,
            DateOnly dateOfWork)
        {
            Expression<Func<WaitingPostPosition, bool>> filter =
                x => x.PostID == postId &&
                     x.DateOfWork == dateOfWork &&
                     x.PostPositionStatus == PostPositionStatus.Waiting;

            Expression<Func<WaitingPostPosition, object>>[] includes =
            {
                x => x.Employee,
                x => x.Employee.EmployeeType,
                x => x.Employee.Department,
                x => x.Employee.Status,
                x => x.Post,
                x => x.Post.Request,
                x => x.Post.Request.RequestSchedules
            };

            var entities = (await _repository.GetAllAsync(filter, includes, pageParam: null).ConfigureAwait(false)).ToList();
            entities = entities
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.CreatedDate ?? DateTime.MinValue)
                .ThenBy(x => x.WaitingPostPositionID)
                .ToList();
            return _mapper.Map<List<WaitingPostPositionResponseDto>>(entities);
        }
        public async Task<List<WaitingPostPositionResponseDto>> GetSpecialEventEmployeeIdsAsync(Guid requestId, DateOnly dateOfWork)
        {
            var entities = (await _repository.GetAllAsync(
                    x => x.RequestID == requestId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == PostPositionStatus.SpecialEvent &&
                         x.ProcessingStatus == ProcessingStatus.Pending,
                    includes: new Expression<Func<WaitingPostPosition, object>>[]
                    {
                        x => x.Employee,
                        x => x.Employee.EmployeeType,
                        x => x.Employee.Department,
                        x => x.Employee.Status,
                    },
                    pageParam: null)
                .ConfigureAwait(false)).ToList();

            entities = entities
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.CreatedDate ?? DateTime.MinValue)
                .ThenBy(x => x.WaitingPostPositionID)
                .ToList();

            return _mapper.Map<List<WaitingPostPositionResponseDto>>(entities);
        }

        public async Task<Dictionary<Guid, WaitingPostPositionResponseDto>> GetPendingSpecialEventRowsByEmployeeAsync(Guid requestId, DateOnly dateOfWork)
        {
            var rows = await GetSpecialEventEmployeeIdsAsync(requestId, dateOfWork).ConfigureAwait(false);

            return (rows ?? new List<WaitingPostPositionResponseDto>())
                .Where(x =>
                    x.EmployeeID != Guid.Empty &&
                    x.WaitingPostPositionID.HasValue &&
                    x.WaitingPostPositionID.Value != Guid.Empty &&
                    x.PostPositionStatus == PostPositionStatus.SpecialEvent &&
                    x.ProcessingStatus == ProcessingStatus.Pending)
                .GroupBy(x => x.EmployeeID)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .OrderBy(x => x.OrderNumber)
                        .ThenBy(x => x.CreatedDate ?? DateTime.MinValue)
                        .ThenBy(x => x.WaitingPostPositionID)
                        .First());
        }

        public async Task<bool> MarkSpecialEventAssignmentFailedAsync(
            Guid waitingPostPositionId,
            string message,
            int? statusCode = null,
            CancellationToken cancellationToken = default)
        {
            return await UpdateProcessingStatusAsync(
                    waitingPostPositionId,
                    ProcessingStatus.Failed,
                    string.IsNullOrWhiteSpace(message) ? "Employee is not eligible for assignment on this date." : message,
                    statusCode,
                    markProcessed: true,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> BulkDeleteForJobAsync(List<Guid> tenureWaitingPositionIds, CancellationToken cancellationToken = default)
        {
            if (tenureWaitingPositionIds == null || tenureWaitingPositionIds.Count == 0)
                return false;

            var ids = tenureWaitingPositionIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return false;

            // Use repository directly (no token-scoped filtering)
            return await _repository
                .BulkDeleteAsync(ids, "WaitingPostPositionID")
                .ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task ReorderWaitingPositionsByGivenOrderNumbersAsync(
            Guid postId,
            DateOnly dateOfWork,
            List<Guid> orderedEmployeeIds,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (orderedEmployeeIds == null || orderedEmployeeIds.Count == 0)
                return;

            var waitingRows = (await _repository.GetAllAsync(
                    x => x.PostID == postId &&
                         x.DateOfWork == dateOfWork &&
                         x.PostPositionStatus == PostPositionStatus.Waiting,
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false)).ToList();

            if (waitingRows.Count == 0)
                return;

            var rowsByEmployee = waitingRows
                .GroupBy(r => r.EmployeeID)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(r => r.OrderNumber).First());

            var newOrder = 1;
            foreach (var empId in orderedEmployeeIds)
            {
                if (!rowsByEmployee.TryGetValue(empId, out var row))
                    continue;

                await UpdateAsync(
                        x => x.WaitingPostPositionID == row.WaitingPostPositionID &&
                             x.PostPositionStatus == PostPositionStatus.Waiting,
                        e =>
                        {
                            e.OrderNumber = newOrder;
                            e.EditedDate = DateTime.UtcNow;
                        })
                    .ConfigureAwait(false);

                newOrder++;
            }
        }
        public async Task<bool> UpdateProcessingStatusAsync(Guid id, ProcessingStatus status, string? message = null, int? statusCode = null, bool markProcessed = false, CancellationToken cancellationToken = default)
        {
            var row = await GetByIdAsync(id);
            if (row == null)
                return false;

            row.ProcessingStatus = status;

            if (message != null)
                row.ResultMessage = message;

            if (statusCode.HasValue)
                row.StatusCode = statusCode.Value;

            if (markProcessed)
                row.ProcessedAt = DateTime.UtcNow;

            row.EditedDate = DateTime.UtcNow;
            await UpsertAsync(row);
            return true;
        }
    }
}

