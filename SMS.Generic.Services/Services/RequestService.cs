using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Exceptions;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Models;
using SMS.Generic.Common.Constants;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using static SMS.Generic.Dtos.Dtos.PostDto;
using SMS.Generic.Common.Enums;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing request operations using the generic service pattern.
    /// </summary>
    public class RequestService : GenericService<RequestDto, Request>, IRequestService
    {
        private readonly IRequestRepository _repository;
        private readonly IRequestTypeService _requestTypeService;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IRequesterService _requesterService;
        private readonly IPostService _postService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISkillService _skillService;
        private readonly IRequestEquipmentRepository _requestEquipmentRepository;
        private readonly IRequestDepartmentFeeRepository _requestDepartmentFeeRepository;
        private readonly IPostEquipmentRepository _postEquipmentRepository;
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IRequestApprovalRoutingService _requestApprovalRoutingService;
        private readonly IApprovalDetailRepository _approvalDetailRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IRoutingGroupService _routingGroupService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IDepartmentService _departmentService;
        private readonly IVehicleApprovalLinkBuilder _vehicleApprovalLinkBuilder;
        private readonly IClosedPostPositionRepository _closedPostPositionRepository;
        private readonly IClosedPostPositionEquipmentRepository _closedPostPositionEquipmentRepository;
        private readonly IDepartmentFeeService _departmentFeeService;
        private readonly IFirebasePushNotificationService _pushNotificationService;
        private readonly IRequestScheduleService _requestScheduleService;
        private readonly IRequesterManagerService _requesterManagerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly IDepartmentInvoiceConfigService _departmentInvoiceConfigService;
        private readonly IPostWorkflowAuditService _postWorkflowAuditService;
        private readonly IPostPositionFlowAuditService _postPositionFlowAuditService;
        private readonly IPostEquipmentService _postEquipmentService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IPostNoteService _postNoteService;
        private readonly IViewDetailsEquipmentRepository _viewDetailsEquipmentRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IPlaceholderService _placeholderService;
        private readonly ILogger<RequestService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestService"/> class.
        /// </summary>
        /// <param name="repository">The request repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="requesterService">The requester service (used to validate RequesterID).</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="skillService">The skill service (used to validate SkillIDs).</param>
        /// <param name="requestEquipmentRepository">The request equipment repository (used to delete vehicle request equipment directly).</param>
        /// <param name="postEquipmentRepository">The post equipment repository (used to delete post equipment linked to the request when flags change).</param>
        /// <param name="departmentEquipmentService">The department equipment service (used to find vehicle department equipment).</param>
        /// <param name="requestApprovalRoutingService">The request approval routing service.</param>
        /// <param name="approvalDetailRepository">The approval detail repository.</param>
        /// <param name="employeeService">The employee service (used to validate employee skills and rank).</param>
        /// <param name="routingGroupService">The routing group service (to fetch approval group email and title).</param>
        /// <param name="emailTemplateService">The email template service (to send Vehicle Approval emails).</param>
        /// <param name="vehicleApprovalLinkBuilder">Builds base URL for approval links in emails.</param>
        public RequestService(
            IRequestRepository repository,
            IRequestTypeService requestTypeService,
            IMapper mapper,
            ITokenParser tokenParser,
            IRequesterService requesterService,
            ILogger<RequestService> logger,
            IPostService postService,
            ISkillService skillService,
            IRequestEquipmentRepository requestEquipmentRepository,
            IRequestDepartmentFeeRepository requestDepartmentFeeRepository,
            IPostEquipmentRepository postEquipmentRepository,
            IDepartmentEquipmentService departmentEquipmentService,
            IRequestApprovalRoutingService requestApprovalRoutingService,
            IApprovalDetailRepository approvalDetailRepository,
            IEmployeeService employeeService,
            IServiceScopeFactory scopeFactory,
            IRoutingGroupService routingGroupService,
            IEmailTemplateService emailTemplateService,
            IDepartmentService departmentService,
            IVehicleApprovalLinkBuilder vehicleApprovalLinkBuilder,
            IClosedPostPositionRepository closedPostPositionRepository,
            IClosedPostPositionEquipmentRepository closedPostPositionEquipmentRepository,
            IDepartmentFeeService departmentFeeService,
            IFirebasePushNotificationService pushNotificationService,
            IRequestScheduleService requestScheduleService,
            IRequesterManagerService requesterManagerService,
            IHttpContextAccessor httpContextAccessor,
            IPostPositionRepository postPositionRepository,
            IPostWorkflowAuditService postWorkflowAuditService,
            IPostPositionFlowAuditService postPositionFlowAuditService,
            IDepartmentInvoiceConfigService departmentInvoiceConfigService,
            IPostEquipmentService postEquipmentService,
            IUploadFileService uploadFileService,
            IPostNoteService postNoteService,
            IViewDetailsEquipmentRepository viewDetailsEquipmentRepository,
            IInvoiceService invoiceService,
            IPlaceholderService placeholderService) : base(repository, mapper, "RequestID", tokenParser)
        {
            _repository = repository;
            _requestTypeService = requestTypeService;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _requesterService = requesterService;
            _postService = postService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _skillService = skillService;
            _requestEquipmentRepository = requestEquipmentRepository;
            _requestDepartmentFeeRepository = requestDepartmentFeeRepository;
            _postEquipmentRepository = postEquipmentRepository ?? throw new ArgumentNullException(nameof(postEquipmentRepository));
            _departmentEquipmentService = departmentEquipmentService;
            _requestApprovalRoutingService = requestApprovalRoutingService ?? throw new ArgumentNullException(nameof(requestApprovalRoutingService));
            _approvalDetailRepository = approvalDetailRepository ?? throw new ArgumentNullException(nameof(approvalDetailRepository));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _scopeFactory = scopeFactory;
            _routingGroupService = routingGroupService ?? throw new ArgumentNullException(nameof(routingGroupService));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _vehicleApprovalLinkBuilder = vehicleApprovalLinkBuilder ?? throw new ArgumentNullException(nameof(vehicleApprovalLinkBuilder));
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _closedPostPositionRepository = closedPostPositionRepository ?? throw new ArgumentNullException(nameof(closedPostPositionRepository));
            _closedPostPositionEquipmentRepository = closedPostPositionEquipmentRepository ?? throw new ArgumentNullException(nameof(closedPostPositionEquipmentRepository));
            _departmentFeeService = departmentFeeService ?? throw new ArgumentNullException(nameof(departmentFeeService));
            _pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            _requestScheduleService = requestScheduleService ?? throw new ArgumentNullException(nameof(requestScheduleService));
            _requesterManagerService = requesterManagerService ?? throw new ArgumentNullException(nameof(requesterManagerService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _postPositionRepository = postPositionRepository ?? throw new ArgumentNullException(nameof(postPositionRepository));
            _departmentInvoiceConfigService = departmentInvoiceConfigService ?? throw new ArgumentNullException(nameof(departmentInvoiceConfigService));
            _postWorkflowAuditService = postWorkflowAuditService ?? throw new ArgumentNullException(nameof(postWorkflowAuditService));
            _postPositionFlowAuditService = postPositionFlowAuditService ?? throw new ArgumentNullException(nameof(postPositionFlowAuditService));
            _postEquipmentService = postEquipmentService;
            _uploadFileService = uploadFileService ?? throw new ArgumentNullException(nameof(uploadFileService));
            _postNoteService = postNoteService ?? throw new ArgumentNullException(nameof(postNoteService));
            _viewDetailsEquipmentRepository = viewDetailsEquipmentRepository
                ?? throw new ArgumentNullException(nameof(viewDetailsEquipmentRepository));
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _placeholderService = placeholderService ?? throw new ArgumentNullException(nameof(placeholderService));
        }

        public override async Task<IEnumerable<RequestDto>> GetAllAsync(System.Linq.Expressions.Expression<Func<Request, bool>>? filter = null, System.Linq.Expressions.Expression<Func<Request, object>>[] includes = null, PageParam pageParam = null)
        {
            var adminScope = AdminRoleScopeHelper.GetScope(_tokenParser);
            if (adminScope.IsRequesterManagerOnly && adminScope.EmployeeId.HasValue)
            {
                var employeeGuid = adminScope.EmployeeId.Value;
                Expression<Func<Request, bool>> requesterManagerFilter = r =>
                    r.RequesterManagerID.HasValue &&
                    r.RequesterManager != null &&
                    (
                        // Always visible to the owner manager who created/owns this request-manager link.
                        r.RequesterManager.EmployeeID == employeeGuid
                        ||
                        // If creator manager marked request sharing ON, expose to all managers under same requester.
                        (
                            r.RequesterManager.IsRequestShared &&
                            r.Requester != null &&
                            r.Requester.RequesterManagers.Any(rm => rm.EmployeeID == employeeGuid)
                        )
                    );

                filter = filter == null
                    ? requesterManagerFilter
                    : ExpressionCombiner.AndAlso(filter, requesterManagerFilter);
            }

            var includesList = includes == null ? new List<System.Linq.Expressions.Expression<Func<Request, object>>>() : includes.ToList();
            includesList.Add(x => x.RequestType);
            includesList.Add(x => x.RequestSchedules);
            var request = (await base.GetAllAsync(filter, includesList.ToArray(), pageParam)).ToList();
            for (var i = 0; i < request.Count; i++)
                request[i].PostCount = (await _postService.GetPostByRequestIdAsync(request[i].RequestID)).TotalRecords;

            return request;
        }

        public async Task<PagedList<RequesterWithRequestsListDto>> GetRequestersWithRequestsAsync(
            PageParam pageParam,
            string? requesterName = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            IReadOnlyCollection<RequestTypes>? requestTypes = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            bool? includeCompleted = null,
            CancellationToken cancellationToken = default)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(Requester.Name);
            requesterName = string.IsNullOrWhiteSpace(requesterName) ? null : requesterName.Trim();
            var deptId = _tokenParser.GetDepartmentID()!.Value;
            var includePast = includeCompleted == true || requestStatuses?.Contains(RequestStatus.Completed) == true;
            var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier;

            var listQuery = new RequesterWorkspaceListQuery
            {
                DepartmentId = deptId,
                RequesterName = requesterName,
                StartDate = startDate,
                EndDate = endDate,
                RequestStatuses = requestStatuses?.ToArray(),
                RequestTypes = requestTypes?.ToArray(),
                IncludePast = includePast
            };

            var pageRequestersSw = Stopwatch.StartNew();
            var (totalRequesters, requesterPage) = await _repository
                .GetPagedRequestersForWorkspaceListAsync(listQuery, pageParam, cancellationToken)
                .ConfigureAwait(false);
            pageRequestersSw.Stop();

            _logger.LogInformation(
                "RequesterWorkspaceList admin page requesters TraceId={TraceId} ElapsedMs={ElapsedMs} TotalRequesters={TotalRequesters} PageCount={PageCount}",
                traceId,
                pageRequestersSw.ElapsedMilliseconds,
                totalRequesters,
                requesterPage.Count);

            if (requesterPage.Count == 0)
                return new PagedList<RequesterWithRequestsListDto>(new List<RequesterWithRequestsListDto>(), pageParam, totalRequesters);

            var pagedRequesterIds = requesterPage.Select(r => r.RequesterId).ToList();

            var requestsSw = Stopwatch.StartNew();
            var (requests, postCounts) = await _repository
                .GetRequestsForWorkspaceListAsync(listQuery, pagedRequesterIds, cancellationToken)
                .ConfigureAwait(false);
            requestsSw.Stop();

            var invoiceSw = Stopwatch.StartNew();
            var unpaidRequesterIds = await _invoiceService
                .GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(pagedRequesterIds, cancellationToken)
                .ConfigureAwait(false);
            invoiceSw.Stop();

            _logger.LogInformation(
                "RequesterWorkspaceList admin load data TraceId={TraceId} RequestsMs={RequestsMs} InvoiceMs={InvoiceMs} RequestRows={RequestRows} RequesterIds={RequesterIds}",
                traceId,
                requestsSw.ElapsedMilliseconds,
                invoiceSw.ElapsedMilliseconds,
                requests.Count,
                pagedRequesterIds.Count);

            if (requests.Count == 0)
                return new PagedList<RequesterWithRequestsListDto>(new List<RequesterWithRequestsListDto>(), pageParam, totalRequesters);

            var requestsByRequesterId = new Dictionary<Guid, List<RequestInRequesterListDto>>();
            foreach (var r in requests)
            {
                var schedules = (r.RequestSchedules ?? Enumerable.Empty<RequestSchedule>())
                    .Where(rs => requestStatuses == null || requestStatuses.Count == 0 || requestStatuses.Contains(rs.RequestStatus))
                    .Where(rs => requestStatuses == null || requestStatuses.Count == 0 || includePast || rs.RequestStatus != RequestStatus.Completed)
                    .OrderBy(s => s.StartDate ?? DateOnly.MaxValue)
                    .ThenBy(s => s.RequestScheduleID)
                    .ToList();

                if (!includePast && requestStatuses is { Count: > 0 } && schedules.Count == 0)
                    continue;

                var row = _mapper.Map<RequestInRequesterListDto>(r);
                row.RequestSchedules = _mapper.Map<List<RequestScheduleInRequesterListDto>>(schedules);
                row.PostCount = postCounts.TryGetValue(r.RequestID, out var count) ? count : 0;
                if (schedules.Count > 0)
                    row.ReleaseStatus = schedules[0].RequestStatus.ToString();

                if (!requestsByRequesterId.TryGetValue(r.RequesterID, out var list))
                {
                    list = new List<RequestInRequesterListDto>();
                    requestsByRequesterId[r.RequesterID] = list;
                }

                list.Add(row);
            }

            var result = new List<RequesterWithRequestsListDto>();
            foreach (var (requesterId, requesterNameValue) in requesterPage)
            {
                if (!requestsByRequesterId.TryGetValue(requesterId, out var rows) || rows.Count == 0)
                    continue;

                rows = rows
                    .OrderBy(x => x.StartDate)
                    .ThenBy(x => x.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                result.Add(new RequesterWithRequestsListDto
                {
                    RequesterId = requesterId,
                    RequesterName = requesterNameValue,
                    RequestsCount = rows.Count,
                    HasUnpaidInvoice = unpaidRequesterIds.Contains(requesterId),
                    Requests = rows,
                });
            }

            return new PagedList<RequesterWithRequestsListDto>(result, pageParam, totalRequesters);
        }

        /// <inheritdoc />
        public async Task<PagedList<RequesterWorkspaceListItemDto>> GetRequesterWorkspaceListAsync(
            RequesterWorkspaceListFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new RequesterWorkspaceListFilterDto();
            var pageParam = filter.PageParam ?? new PageParam();
            var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier;
            var totalSw = Stopwatch.StartNew();

            if (filter.IsDailyWorkspace)
            {
                if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                    pageParam.SortBy = "requesterName";

                var summaryFilter = _mapper.Map<RequesterRequestSummaryFilterDto>(filter);
                var includeRequestDetails = !string.IsNullOrWhiteSpace(summaryFilter.SearchText);

                var managerSw = Stopwatch.StartNew();
                var requesterManagerIds = await ResolveRequesterManagerIdsForWorkspaceAsync().ConfigureAwait(false);
                managerSw.Stop();

                var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;
                DateTime? shiftWindowStart = null;
                DateTime? shiftWindowEnd = null;
                if (summaryFilter.IsShiftStartingWithin30Minutes)
                {
                    var departmentNow = GetCurrentDepartmentDateTime();
                    shiftWindowStart = departmentNow.AddMinutes(-30);
                    shiftWindowEnd = departmentNow.AddMinutes(30);
                }

                var summariesSw = Stopwatch.StartNew();
                var summariesPage = await _repository.GetRequesterRequestSummariesPagedAsync(
                    departmentId,
                    pageParam,
                    summaryFilter.RequesterName,
                    summaryFilter.StartDate,
                    summaryFilter.EndDate,
                    summaryFilter.RequestTypes,
                    summaryFilter.RequestStatuses,
                    summaryFilter.ReleaseAs,
                    requesterManagerIds,
                    summaryFilter.ApplyDateOnSchedules,
                    summaryFilter.SearchText,
                    shiftWindowStart,
                    shiftWindowEnd,
                    cancellationToken).ConfigureAwait(false);
                summariesSw.Stop();

                var items = _mapper.Map<List<RequesterWorkspaceListItemDto>>(summariesPage.Data ?? new List<RequesterRequestSummaryDto>());
                if (!includeRequestDetails)
                {
                    foreach (var item in items)
                        item.Requests = null;
                }

                totalSw.Stop();
                _logger.LogInformation(
                    "RequesterWorkspaceList daily TraceId={TraceId} ElapsedMs={ElapsedMs} ManagerMs={ManagerMs} SummariesMs={SummariesMs} " +
                    "TotalRecords={TotalRecords} DataRows={DataRows} IncludeRequestDetails={IncludeRequestDetails}",
                    traceId,
                    totalSw.ElapsedMilliseconds,
                    managerSw.ElapsedMilliseconds,
                    summariesSw.ElapsedMilliseconds,
                    summariesPage.TotalRecords,
                    items.Count,
                    includeRequestDetails);

                return new PagedList<RequesterWorkspaceListItemDto>(items, pageParam, summariesPage.TotalRecords);
            }

            var withRequests = await GetRequestersWithRequestsAsync(
                pageParam,
                filter.RequesterName,
                filter.RequestStatuses,
                filter.RequestTypes,
                filter.StartDate,
                filter.EndDate,
                filter.IncludeCompleted,
                cancellationToken).ConfigureAwait(false);

            var data = _mapper.Map<List<RequesterWorkspaceListItemDto>>(withRequests.Data ?? Enumerable.Empty<RequesterWithRequestsListDto>());
            totalSw.Stop();

            _logger.LogInformation(
                "RequesterWorkspaceList admin TraceId={TraceId} ElapsedMs={ElapsedMs} TotalRecords={TotalRecords} DataRows={DataRows} NestedRequests={NestedRequests}",
                traceId,
                totalSw.ElapsedMilliseconds,
                withRequests.TotalRecords,
                data.Count,
                data.Sum(r => r.Requests?.Count ?? 0));

            return new PagedList<RequesterWorkspaceListItemDto>(data, pageParam, withRequests.TotalRecords);
        }

        private async Task<IReadOnlyCollection<Guid>?> ResolveRequesterManagerIdsForWorkspaceAsync()
        {
            if (_tokenParser.IsDepartmentAdmin())
                return null;

            var employeeId = _tokenParser.GetEmployeeID();
            if (!employeeId.HasValue)
                return Array.Empty<Guid>();

            return await _requesterManagerService
                .GetRequesterManagerIdsByEmployeeIdAsync(employeeId.Value)
                .ConfigureAwait(false);
        }

        public async Task<PagedList<RequestInRequesterListDto>> GetRequestsByRequesterFilterAsync(
            RequesterRequestsFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var pageParam = filter.PageParam ?? new PageParam();
            IReadOnlyCollection<Guid>? requesterManagerIds = null;

            if (!_tokenParser.IsDepartmentAdmin())
            {
                var employeeId = _tokenParser.GetEmployeeID();
                if (!employeeId.HasValue)
                    return new RequesterRequestsResponseDto(pageParam);

                requesterManagerIds = await _requesterManagerService
                    .GetRequesterManagerIdsAsync(filter.RequesterId, employeeId.Value)
                    .ConfigureAwait(false);

                if (requesterManagerIds.Count == 0)
                    return new RequesterRequestsResponseDto(pageParam);
            }

            var departmentId = _tokenParser.GetDepartmentID().Value;
            var (requests, totalRecords, totalPostCount) = await _repository.GetRequestsByRequesterAsync(
                filter.RequesterId,
                departmentId,
                filter.StartDate,
                filter.EndDate,
                filter.RequestTypes,
                filter.RequestStatuses,
                filter.ReleaseAs,
                requesterManagerIds,
                filter.Search,
                pageParam,
                filter.ApplyDateOnSchedules,
                cancellationToken).ConfigureAwait(false);

            var isClearOfOverdueUnpaid = await _invoiceService
                .IsRequesterClearOfOverdueUnpaidInvoicesAsync(filter.RequesterId, cancellationToken)
                .ConfigureAwait(false);

            var rows = _mapper.Map<List<RequestInRequesterListDto>>(requests);
            return new RequesterRequestsResponseDto(
                rows,
                pageParam,
                totalRecords,
                hasUnpaidInvoice: !isClearOfOverdueUnpaid,
                totalRequestCount: (int)totalRecords,
                totalPostCount: totalPostCount);
        }

        /// <inheritdoc />
        public async Task<PagedList<RequestInRequesterListDto>> GetRequestsWithLocationsByRequesterIdAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default)
        {
            var pageParam = new PageParam();
            if (requesterId == Guid.Empty)
                return new RequesterRequestsResponseDto(pageParam);

            IReadOnlyCollection<Guid>? requesterManagerIds = null;

            if (!_tokenParser.IsDepartmentAdmin())
            {
                var employeeId = _tokenParser.GetEmployeeID();
                if (!employeeId.HasValue)
                    return new RequesterRequestsResponseDto(pageParam);

                requesterManagerIds = await _requesterManagerService
                    .GetRequesterManagerIdsAsync(requesterId, employeeId.Value)
                    .ConfigureAwait(false);

                if (requesterManagerIds.Count == 0)
                    return new RequesterRequestsResponseDto(pageParam);
            }

            var departmentNow = GetCurrentDepartmentDateTime();
            var shiftWindowStart = departmentNow.AddMinutes(-30);
            var shiftWindowEnd = departmentNow.AddMinutes(30);
            var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;

            var requests = await _repository.GetRequestsWithLocationsByRequesterAsync(
                requesterId,
                departmentId,
                shiftWindowStart,
                shiftWindowEnd,
                requesterManagerIds,
                cancellationToken).ConfigureAwait(false);

            var isClearOfOverdueUnpaid = await _invoiceService
                .IsRequesterClearOfOverdueUnpaidInvoicesAsync(requesterId, cancellationToken)
                .ConfigureAwait(false);

            // 1. Filter eligible positions in the active shift window (30 mins before start until 30 mins after end)
            bool IsEligiblePosition(PostPosition pp, Post post)
            {
                if (pp.EmployeeID is not { } empId || empId == Guid.Empty) return false;
                if (pp.PostPositionStatus is PostPositionStatus.Cancelled or PostPositionStatus.Closed or PostPositionStatus.ClosedAndSplit or PostPositionStatus.Declined) return false;
                if (pp.DateOfWork is not { } workDate) return false;

                var shiftStart = workDate.ToDateTime(pp.StartTime ?? post.PostStartTime);
                var shiftEnd = workDate.ToDateTime(pp.EndTime ?? post.PostEndTime);
                if (shiftEnd <= shiftStart) shiftEnd = shiftEnd.AddDays(1);

                return departmentNow >= shiftStart.AddMinutes(-30) && departmentNow <= shiftEnd.AddMinutes(30);
            }

            var eligiblePositions = (
                from req in requests
                from post in req.Posts ?? Enumerable.Empty<Post>()
                from pp in post.PostPositions ?? Enumerable.Empty<PostPosition>()
                where IsEligiblePosition(pp, post)
                select (Request: req, Post: post, Position: pp)
            ).ToList();

            if (eligiblePositions.Count == 0)
            {
                return new RequesterRequestsResponseDto(
                    Array.Empty<RequestInRequesterListDto>(),
                    pageParam,
                    totalRecords: 0,
                    hasUnpaidInvoice: !isClearOfOverdueUnpaid,
                    totalRequestCount: 0,
                    totalPostCount: 0);
            }

            // 2. Fetch and map latest locations with O(1) lookups
            var targetPosIds = eligiblePositions.Select(x => x.Position.PostPositionID).Distinct().ToList();
            var targetEmpIds = eligiblePositions.Select(x => x.Position.EmployeeID!.Value).Distinct().ToList();

            var locations = await _repository.GetLatestLocationsForPositionsAsync(
                targetPosIds,
                targetEmpIds,
                cancellationToken).ConfigureAwait(false);

            var locDtos = locations.Select(l => (l.PostPositionID, l.EmployeeID, Dto: _mapper.Map<EmployeePostLocationItemDto>(l))).ToList();
            var locsByPos = locDtos.Where(x => x.PostPositionID != Guid.Empty).ToLookup(x => x.PostPositionID, x => x.Dto);
            var locsByEmp = locDtos.Where(x => x.EmployeeID != Guid.Empty).ToLookup(x => x.EmployeeID, x => x.Dto);

            // 3. Construct hierarchy using AutoMapper
            var rows = eligiblePositions
                .GroupBy(x => x.Request)
                .Select(reqGroup =>
                {
                    var requestDto = _mapper.Map<RequestInRequesterListDto>(reqGroup.Key);
                    requestDto.Posts = reqGroup
                        .GroupBy(x => x.Post)
                        .Select(postGroup =>
                        {
                            var postDto = _mapper.Map<PostWithEmployeeLocationDto>(postGroup.Key);
                            postDto.PostPositions = postGroup.Select(x =>
                            {
                                var posDto = _mapper.Map<PostPositionEmployeeLocationDto>(x.Position);
                                posDto.StartTime ??= postGroup.Key.PostStartTime;
                                posDto.EndTime ??= postGroup.Key.PostEndTime;
                                posDto.EmployeePostLocations = locsByPos[x.Position.PostPositionID].ToList() is { Count: > 0 } pLocs
                                    ? pLocs
                                    : locsByEmp[x.Position.EmployeeID!.Value].ToList();
                                return posDto;
                            }).ToList();
                            return postDto;
                        }).ToList();

                    requestDto.PostCount = requestDto.Posts.Count;
                    return requestDto;
                })
                .ToList();

            return new RequesterRequestsResponseDto(
                rows,
                pageParam,
                rows.Count,
                hasUnpaidInvoice: !isClearOfOverdueUnpaid,
                totalRequestCount: rows.Count,
                totalPostCount: rows.Sum(r => r.PostCount));
        }

        /// <inheritdoc />
        public Task<PagedList<RequestInRequesterListDto>> GetRequestsWithLocationsByRequesterFilterAsync(
            RequesterRequestsFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            return GetRequestsWithLocationsByRequesterIdAsync(filter?.RequesterId ?? Guid.Empty, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
            RequesterRequestSummaryFilterDto filter,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default)
        {
            filter ??= new RequesterRequestSummaryFilterDto();

            if (filter.IsShiftStartingWithin30Minutes)
            {
                var departmentNow = GetCurrentDepartmentDateTime();
                var shiftWindowStart = departmentNow.AddMinutes(-30);
                var shiftWindowEnd = departmentNow.AddMinutes(30);

                return await GetRequesterRequestSummariesByShiftWindowAsync(
                    filter,
                    shiftWindowStart,
                    shiftWindowEnd,
                    requesterManagerIds,
                    cancellationToken).ConfigureAwait(false);
            }

            return await _repository.GetRequesterRequestSummariesAsync(
                _tokenParser.GetDepartmentID() ?? Guid.Empty,
                filter.RequesterName,
                filter.StartDate,
                filter.EndDate,
                filter.RequestTypes,
                filter.RequestStatuses,
                filter.ReleaseAs,
                requesterManagerIds,
                filter.ApplyDateOnSchedules,
                filter.SearchText,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowAsync(
            RequesterRequestSummaryFilterDto filter,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default)
        {
            filter ??= new RequesterRequestSummaryFilterDto();

            return await _repository.GetRequesterRequestSummariesByShiftWindowAsync(
                _tokenParser.GetDepartmentID() ?? Guid.Empty,
                shiftWindowStart,
                shiftWindowEnd,
                filter.RequesterName,
                filter.StartDate,
                filter.EndDate,
                filter.RequestTypes,
                filter.RequestStatuses,
                filter.ReleaseAs,
                requesterManagerIds,
                filter.ApplyDateOnSchedules,
                filter.SearchText,
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<IEnumerable<RequestDto>> SearchRequests(DateOnly searchStartDate, DateOnly searchEndDate, Guid departmentId, ReleaseAs release,DateTime currentDateTime)
        {
            var entities = await _repository.SearchRequests(searchStartDate, searchEndDate, departmentId, release, currentDateTime);
            return _mapper.Map<IEnumerable<RequestDto>>(entities);
        }

        private async Task<RequestDto> GetRequestByPostId(Guid postId, Guid departmentId)
        {
            var entities = await _repository.GetRequestByPostId(postId, departmentId);
            return _mapper.Map<RequestDto>(entities);
        }

        public async Task<RequestDto> CloneRequestAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("Request ID cannot be empty.", nameof(requestId));

            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

            var sourceRequest = await _repository.GetRequestForCloneAsync(requestId, departmentId.Value).ConfigureAwait(false);
            if (sourceRequest == null)
                throw new KeyNotFoundException($"Request with ID {requestId} not found.");

            var sourceDto = _mapper.Map<RequestDto>(sourceRequest);
            sourceDto.PrepareForCloneAsNewRequestSeed();

            // BulkUpsertAsync skips UpsertAsync and would persist the source ProjectNumber; UpsertAsync assigns the next number like a new request.
            var clonedRequest = await UpsertAsync(sourceDto).ConfigureAwait(false);

            if (sourceRequest.RequestType != null)
            {
                var clonedRequestType = _mapper.Map<RequestTypeDto>(sourceRequest.RequestType);
                clonedRequestType.PrepareForCloneAsNewRequest(clonedRequest.RequestID);
                await _requestTypeService.UpsertAsync(clonedRequestType).ConfigureAwait(false);
            }

            var sourceRequestEquipments = (await _requestEquipmentRepository.GetAllAsync(
                re => re.RequestID == requestId,
                includes: null,
                pageParam: null).ConfigureAwait(false))
                .ToList();

            if (sourceRequestEquipments.Count > 0)
            {
                var clonedRequestEquipments = sourceRequestEquipments
                    .Select(re => new RequestEquipment
                    {
                        RequestEquipmentID = Guid.Empty,
                        RequestID = clonedRequest.RequestID,
                        DepartmentEquipmentID = re.DepartmentEquipmentID,
                        EquipmentPerPost = re.EquipmentPerPost,
                        EquipmentQuantity = re.EquipmentQuantity,
                        IsVehicle = re.IsVehicle,
                        StartDate = re.StartDate,
                        EndDate = re.EndDate
                    })
                    .ToList();

                await _requestEquipmentRepository
                    .BulkUpsertAsync(clonedRequestEquipments, "RequestEquipmentID")
                    .ConfigureAwait(false);
            }

            if (sourceRequest.EnableFees)
            {
                var sourceRequestFees = (await _requestDepartmentFeeRepository.GetAllAsync(
                    r => r.RequestID == requestId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false))
                    .ToList();

                if (sourceRequestFees.Count > 0)
                {
                    var clonedRequestFees = sourceRequestFees
                        .Select(r => new RequestDepartmentFee
                        {
                            RequestDepartmentFeeID = Guid.Empty,
                            RequestID = clonedRequest.RequestID,
                            DepartmentFeeID = r.DepartmentFeeID
                        })
                        .ToList();

                    await _requestDepartmentFeeRepository
                        .BulkUpsertAsync(clonedRequestFees, "RequestDepartmentFeeID")
                        .ConfigureAwait(false);
                }
            }

            var sourcePosts = (sourceRequest.Posts ?? new List<Post>())
                .Where(p => p.PostID != Guid.Empty)
                .OrderBy(p => p.CreatedDate)
                .ToList();

            if (!sourcePosts.Any())
                return clonedRequest;

            // Two-phase clone keeps parent-child hierarchy inside the newly cloned request.
            var sourcePostIdsInOrder = new List<Guid>(sourcePosts.Count);
            var oldParentByOldPostId = new Dictionary<Guid, Guid?>(sourcePosts.Count);
            var sourceBlockDatesByOldPostId = new Dictionary<Guid, List<BlockDateDto>>(sourcePosts.Count);
            var postSeedDtos = new List<PostDto>(sourcePosts.Count);

            foreach (var sourcePost in sourcePosts)
            {
                var oldPostId = sourcePost.PostID;
                sourcePostIdsInOrder.Add(oldPostId);
                oldParentByOldPostId[oldPostId] = sourcePost.PostParentID;
                sourceBlockDatesByOldPostId[oldPostId] = (sourcePost.BlockDates ?? new List<BlockDate>())
                    .Select(b => _mapper.Map<BlockDateDto>(b))
                    .ToList();

                var clonePostDto = _mapper.Map<PostDto>(sourcePost);
                clonePostDto.PrepareForClone(clonedRequest.RequestID);
                postSeedDtos.Add(clonePostDto);
            }

            var seededPosts = (await _postService.BulkUpsertAsync(postSeedDtos).ConfigureAwait(false)).ToList();

            var oldToNewPostIdMap = new Dictionary<Guid, Guid>(sourcePostIdsInOrder.Count);
            for (var i = 0; i < sourcePostIdsInOrder.Count; i++)
            {
                var newPostId = seededPosts[i].PostID;
                if (!newPostId.HasValue || newPostId.Value == Guid.Empty)
                    continue;

                oldToNewPostIdMap[sourcePostIdsInOrder[i]] = newPostId.Value;
            }

            var postsToReparent = new List<PostDto>();
            for (var i = 0; i < sourcePostIdsInOrder.Count; i++)
            {
                var oldPostId = sourcePostIdsInOrder[i];
                if (!oldParentByOldPostId.TryGetValue(oldPostId, out var oldParentId) || !oldParentId.HasValue)
                    continue;

                if (!oldToNewPostIdMap.TryGetValue(oldParentId.Value, out var newParentId))
                    continue;

                var seededPost = seededPosts[i];
                seededPost.PostParentID = newParentId;
                postsToReparent.Add(seededPost);
            }

            if (postsToReparent.Count > 0)
            {
                await _postService.BulkUpsertAsync(postsToReparent).ConfigureAwait(false);
            }

            await _postService.ClonePostEquipmentAsync(oldToNewPostIdMap).ConfigureAwait(false);
            await _postService.ClonePostCallSignsAsync(oldToNewPostIdMap).ConfigureAwait(false);

            // Clone block dates for newly created posts (these are stored in a separate table).
            using (var blockScope = _scopeFactory.CreateScope())
            {
                var blockDateService = blockScope.ServiceProvider.GetRequiredService<IBlockDateService>();
                var clonedBlockDates = new List<BlockDateDto>();

                foreach (var oldToNewPostId in oldToNewPostIdMap)
                {
                    if (!sourceBlockDatesByOldPostId.TryGetValue(oldToNewPostId.Key, out var sourceBlockDates) || sourceBlockDates.Count == 0)
                        continue;

                    foreach (var sourceBlockDate in sourceBlockDates)
                    {
                        clonedBlockDates.Add(new BlockDateDto
                        {
                            BlockDateId = Guid.Empty,
                            RequestID = clonedRequest.RequestID,
                            PostID = oldToNewPostId.Value,
                            StartDateTime = sourceBlockDate.StartDateTime,
                            EndDateTime = sourceBlockDate.EndDateTime
                        });
                    }
                }

                if (clonedBlockDates.Count > 0)
                {
                    await blockDateService.BulkUpsertAsync(clonedBlockDates).ConfigureAwait(false);
                }
            }

            return clonedRequest;
        }
        public override async Task<RequestDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id, "RequestID", x => x.RequestType);
            var result = _mapper.Map<RequestDto>(entity);
            var schedules = await _requestScheduleService.GetAllAsync(x => x.RequestId == id);
            result.RequestSchedules = schedules.ToList();

            var selectedFees = await _requestDepartmentFeeRepository.GetAllAsync(
                r => r.RequestID == id, null, null).ConfigureAwait(false);
            result.SelectedDepartmentFeeIds = selectedFees.Select(r => r.DepartmentFeeID).ToList();

            return result;
        }

        /// <summary>
        /// Deletes a request and all dependent child data required by FK-restrict relationships.
        /// </summary>
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is not Guid requestId || requestId == Guid.Empty)
            {
                return await base.DeleteAsync(id).ConfigureAwait(false);
            }

            var existing = await GetByIdAsync(requestId).ConfigureAwait(false);
            if (existing == null)
            {
                return false;
            }

            using var scope = _scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var waitingPostPositionService = sp.GetRequiredService<IWaitingPostPositionService>();
            var requestNoteService = sp.GetRequiredService<IRequestNoteService>();
            var permitApprovalRoutingService = sp.GetRequiredService<IPermitApprovalRoutingService>();
            var invoiceService = sp.GetRequiredService<IInvoiceService>();
            var requestEquipmentService = sp.GetRequiredService<IRequestEquipmentService>();
            var approvalDetailService = sp.GetRequiredService<IApprovalDetailService>();

            // Delete post tree first because Post -> Request is Restrict and post deletion already handles its own descendants/dependents.
            var requestPosts = (await _postService.GetAllAsync(
                p => p.RequestID == requestId,
                includes: null,
                pageParam: null).ConfigureAwait(false)).ToList();

            if (requestPosts.Count > 0)
            {
                var requestPostIds = requestPosts
                    .Where(p => p.PostID.HasValue && p.PostID.Value != Guid.Empty)
                    .Select(p => p.PostID!.Value)
                    .ToHashSet();

                var rootPostIds = requestPosts
                    .Where(p => p.PostID.HasValue && p.PostID.Value != Guid.Empty)
                    .Where(p => !p.PostParentID.HasValue || !requestPostIds.Contains(p.PostParentID.Value))
                    .Select(p => p.PostID!.Value)
                    .Distinct()
                    .ToList();

                foreach (var rootPostId in rootPostIds)
                {
                    await _postService.DeleteAsync(rootPostId).ConfigureAwait(false);
                }
            }

            // Request-owned rows not removed by post soft-delete (via GenericService.UpdateAsync -> repository).
            await waitingPostPositionService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await requestNoteService.UpdateAsync(
                x => x.RequestId == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await _requestScheduleService.UpdateAsync(
                x => x.RequestId == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await _requestTypeService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await requestEquipmentService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await _requestDepartmentFeeRepository.DeleteByFilterAsync(r => r.RequestID == requestId)
                .ConfigureAwait(false);

            await approvalDetailService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await _requestApprovalRoutingService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await permitApprovalRoutingService.UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await invoiceService.UpdateAsync(
                x => x.RequestId == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            await UpdateAsync(
                x => x.RequestID == requestId,
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);
            return true;
        }

        public async Task<ReleaseAs> GetLatestReleaseAsForRequestAsync(Guid requestId)
        {
            return await _repository.GetLatestReleaseAsForRequestAsync(requestId).ConfigureAwait(false);
        }

        public async Task<List<EmployeeDto>> GetAssignableEmployeesForRequestAsync(Guid requestId, string? searchTerm = null)
        {
            var request = await GetByIdAsync(requestId).ConfigureAwait(false);
            if (request == null)
                return new List<EmployeeDto>();

            var releaseAs = await _repository.GetLatestReleaseAsForRequestAsync(requestId).ConfigureAwait(false);
            var departmentEmployees = (await _employeeService.GetAllAsync(
                filter: e => e.DepartmentID == request.DepartmentID && e.IsApproved,
                includes: null,
                pageParam: null).ConfigureAwait(false)).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var tokens = searchTerm.ToLower().Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                departmentEmployees = departmentEmployees.Where(e =>
                    tokens.All(t => new[] { e.FirstName, e.LastName, e.BadgeNumber, e.Username }
                        .Any(v => v?.ToLower().Replace(",", "").Contains(t) == true))).ToList();
            }

            List<EmployeeDto> SortByLastName(IEnumerable<EmployeeDto> employees) =>
                employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

            // OPEN: always show full department employee pool.
            if (releaseAs == ReleaseAs.Open)
                return SortByLastName(departmentEmployees);

            // TENURE (admin-created): full department employee pool.
            if (releaseAs == ReleaseAs.TenureBased && (!request.RequesterID.HasValue || !request.RequesterManagerID.HasValue))
                return SortByLastName(departmentEmployees);

            // For Tenure/Managed/Preferred with requester-manager context:
            // creator manager + managers with IsEmployeeShared.
            if (!request.RequesterID.HasValue || !request.RequesterManagerID.HasValue)
                return new List<EmployeeDto>();

            var creatorManagerId = request.RequesterManagerID.Value;
            var requesterId = request.RequesterID.Value;

            var managerScopes = (await _requesterManagerService.GetAllAsync(
                    filter: rm =>
                        rm.RequesterID == requesterId &&
                        (rm.RequesterManagerID == creatorManagerId || rm.IsEmployeeShared),
                    includes: new Expression<Func<RequesterManager, object>>[]
                    {
                        rm => rm.RequesterEmployees
                    },
                    pageParam: null).ConfigureAwait(false))
                .ToList();

            List<Guid> allowedEmployeeIds = departmentEmployees
                .Where(e => e.EmployeeID.HasValue)
                .Select(e => e.EmployeeID!.Value)
                .Where(employeeId => managerScopes.Any(m => m.IsEmployeeEligibleForReleaseAs(employeeId, releaseAs)))
                .Distinct()
                .ToList();

            if (allowedEmployeeIds.Count == 0)
                return new List<EmployeeDto>();

            return SortByLastName(
                departmentEmployees.Where(e => e.EmployeeID.HasValue && allowedEmployeeIds.Contains(e.EmployeeID.Value)));
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessDueRequestDto> GetTenureBasedRequestsDueForProcessingAsync(DateTime asOfUtc)
        {
            return await _repository.GetTenureBasedRequestsDueForProcessingAsync(asOfUtc).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessDueRequestDto> GetPreferredRequestsDueForProcessingAsync(DateTime asOfUtc)
        {
            return await _repository.GetPreferredRequestsDueForProcessingAsync(asOfUtc).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<SuccessResult<InvoiceDto>> GetRequestDataForInvoiceAsync(string projectNumber, DateOnly startDate, DateOnly endDate, int requestCount = 1)
        {
            if (string.IsNullOrWhiteSpace(projectNumber) || projectNumber.Length < 5)
                throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND));

            var departmentIdFromToken = _tokenParser.GetDepartmentID();
            if (!departmentIdFromToken.HasValue || departmentIdFromToken.Value == Guid.Empty)
                throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND));

            var yearStr = projectNumber.Substring(0, 4);
            var numberStr = projectNumber.Length > 4 ? projectNumber.Substring(4) : string.Empty;
            if (!int.TryParse(yearStr, out var fiscalYear) || string.IsNullOrEmpty(numberStr))
                throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND));

            var request = await _repository.GetDataForInvoice(
                fiscalYear, numberStr, startDate, endDate, departmentIdFromToken.Value).ConfigureAwait(false);

            if (request == null)
                throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND));

            var requestName = !string.IsNullOrWhiteSpace(request.Title)
                ? request.Title
                : projectNumber;
            var (messageStartDate, messageEndDate) = GetRequestMessageDateRange(request, startDate, endDate);
            var payloadStartDate = messageStartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var payloadEndDate = messageEndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            // Validation: fail if any post positions in range are still in Applied status
            var hasAppliedPostsInRange = request.Posts?
                .SelectMany(p => p.PostPositions ?? Enumerable.Empty<PostPosition>())
                .Any(pp =>
                    pp.DateOfWork >= startDate &&
                    pp.DateOfWork <= endDate &&
                    (!pp.IsPlaceholder || pp.EnableInvoiceAndPayroll) &&
                    (pp.PostPositionStatus == PostPositionStatus.Assigned || pp.PostPositionStatus == PostPositionStatus.Unclosed)) == true;

            if (hasAppliedPostsInRange)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_NOT_CLOSED_FOR_DATE_RANGE, requestName, projectNumber, payloadStartDate, payloadEndDate));
            }

            var department = await _departmentService.GetByIdAsync(departmentIdFromToken.Value).ConfigureAwait(false);

            var requestDto = _mapper.Map<RequestDto>(request);
            var orderedPosts = _postService.BuildPostHierarchy(requestDto.Posts?
                .Where(p => p.PostPositions.Any(pp =>
                    pp.PostPositionStatus != PostPositionStatus.Unclosed &&
                    pp.IsEligibleForInvoicing()))
                .ToList() ?? new List<PostDto>());
            var requester = requestDto.Requester;
            var billingRequester = requester;
            if (requester?.IsChild == true && requester.ParentID.HasValue && requester.ParentID.Value != Guid.Empty)
            {
                var parentRequester = await _requesterService.GetByIdAsync(requester.ParentID.Value).ConfigureAwait(false);
                if (parentRequester != null)
                    billingRequester = parentRequester;
            }

            var requestAddress = AddressFormatHelper.BuildSingleLine(
                billingRequester?.InvoiceAddress,
                billingRequester?.InvoiceAddress2,
                billingRequester?.InvoiceCity,
                billingRequester?.InvoiceState,
                billingRequester?.InvoiceZip);

            (string? billingFromName, string? billingFromAddress, string? billingFromPhone, string? billingFromEmail, string? billingFromContact) =
                GetBillingFromDepartment(department);

            var allPostPositions = orderedPosts
                .SelectMany(p => p.PostPositions ?? Enumerable.Empty<PostPositionDto>())
                .Where(pp => pp.PostPositionID.HasValue && pp.IsEligibleForInvoicing())
                .ToList();
            var postPositionIds = allPostPositions.Select(pp => pp.PostPositionID!.Value).Distinct().ToList();

            var closedPostPositions = postPositionIds.Count == 0
                ? new List<ClosedPostPositionDto>()
                : _mapper.Map<List<ClosedPostPositionDto>>(
                    await _closedPostPositionRepository.GetAllAsync(cpp => postPositionIds.Contains(cpp.PostPositionID), null).ConfigureAwait(false));

            var closedByPostPosition = closedPostPositions
                .Where(x => x.Status != ClosedPostPositionStatus.Unclosed)
                .GroupBy(cpp => cpp.PostPositionID)
                .ToDictionary(g => g.Key, g => g.First());

            var closedIds = closedPostPositions.Select(cpp => cpp.ClosedPostPositionID).Distinct().ToList();
            var closedEquipments = closedIds.Count == 0
                ? new List<ClosedPostPositionEquipmentDto>()
                : _mapper.Map<List<ClosedPostPositionEquipmentDto>>(
                    await _closedPostPositionEquipmentRepository.GetAllAsync(cpe => closedIds.Contains(cpe.ClosedPostPositionID), null).ConfigureAwait(false));

            var departmentEquipmentIds = closedEquipments.Select(cpe => cpe.DepartmentEquipmentID).Distinct().ToList();
            var departmentEquipments = departmentEquipmentIds.Count == 0
                ? new List<DepartmentEquipmentDto>()
                : (await _departmentEquipmentService.GetAllAsync(de => departmentEquipmentIds.Contains(de.DepartmentEquipmentID), new Expression<Func<DepartmentEquipment, object>>[]
        {
            x => x.EquipmentType
        }).ConfigureAwait(false)).ToList();
            var deptEquipDict = departmentEquipments.ToDictionary(de => de.DepartmentEquipmentID);

            var employeeIds = allPostPositions.Select(pp => pp.EmployeeID).Distinct().ToList();
            var employees = employeeIds.Count == 0
                ? Enumerable.Empty<EmployeeDto>()
                : await _employeeService.GetAllAsync(e => employeeIds.Contains(e.EmployeeID)).ConfigureAwait(false);
            var employeeDict = employees
                .Where(e => e.EmployeeID.HasValue)
                .ToDictionary(e => e.EmployeeID!.Value);

            var enableLiquorLicenseTracking = false;
            IReadOnlyDictionary<DateOnly, IReadOnlyList<HolidayDayDto>> holidaysByDate =
                new Dictionary<DateOnly, IReadOnlyList<HolidayDayDto>>();
            IReadOnlyDictionary<Guid, EmployeeTypeDto> employeeTypesById =
                new Dictionary<Guid, EmployeeTypeDto>();

            if (departmentIdFromToken.Value != Guid.Empty)
            {
                using var scope = _scopeFactory.CreateScope();
                var departmentConfigurationService = scope.ServiceProvider.GetRequiredService<IDepartmentConfigurationService>();
                var departmentConfiguration = await departmentConfigurationService
                    .GetByDepartmentIdAsync(departmentIdFromToken.Value)
                    .ConfigureAwait(false);
                enableLiquorLicenseTracking = departmentConfiguration?.EnableLiquorLicenseTracking == true;

                var holidayDayService = scope.ServiceProvider.GetRequiredService<IHolidayDayService>();
                var employeeTypeService = scope.ServiceProvider.GetRequiredService<IEmployeeTypeService>();
                var deptId = departmentIdFromToken.Value;

                var holidays = (await holidayDayService.GetAllAsync(h =>
                        h.DepartmentID == deptId && h.Date.HasValue && DateOnly.FromDateTime(h.Date.Value) >= startDate && DateOnly.FromDateTime(h.Date.Value) <= endDate).ConfigureAwait(false)).ToList();

                holidaysByDate = holidays
                    .Where(h => h.Date.HasValue)
                    .GroupBy(h => DateOnly.FromDateTime(h.Date!.Value))
                    .ToDictionary(g => g.Key, g => (IReadOnlyList<HolidayDayDto>)g.ToList());

                var employeeTypeIds = orderedPosts.Select(p => p.EmployeeTypeID).Where(id => id.HasValue && id.Value != Guid.Empty).Select(id => id!.Value).Distinct().ToList();

                if (employeeTypeIds.Count > 0)
                {
                    var employeeTypes = await employeeTypeService.GetAllAsync(et => employeeTypeIds.Contains(et.EmployeeTypeID)).ConfigureAwait(false);
                    employeeTypesById = employeeTypes.ToDictionary(et => et.EmployeeTypeID);
                }
            }

            var invoiceDetails = new List<InvoiceDetailDto>();
            var eligiblePostIds = new HashSet<Guid>();
            var eligibleAssignmentCount = 0;
            var closedWithZeroHoursCount = 0;
            string? zeroHoursEmployeeName = null;
            foreach (var postDto in orderedPosts)
            {
                foreach (var pp in postDto.PostPositions ?? new List<PostPositionDto>())
                {
                    if (!pp.IsEligibleForInvoicing())
                        continue;

                    eligibleAssignmentCount++;
                    var closed = pp.PostPositionID.HasValue && closedByPostPosition.TryGetValue(pp.PostPositionID.Value, out var c) ? c : null;
                    if (closed != null && (closed.HoursWorked ?? 0) <= 0)
                    {
                        closedWithZeroHoursCount++;
                        zeroHoursEmployeeName ??= GetAssignmentEmployeeName(pp, employeeDict);
                        continue;
                    }

                    EmployeeTypeDto? postEmployeeType = postDto.EmployeeTypeID.HasValue && employeeTypesById.TryGetValue(postDto.EmployeeTypeID.Value, out var employeeType) ? employeeType : null;
                    var detail = InvoiceDetailDto.FromPostPosition(pp, startDate, endDate, closed, closedEquipments, deptEquipDict, employeeDict, requestDto, postDto, enableLiquorLicenseTracking, holidaysByDate, postEmployeeType);
                    if (detail != null)
                    {
                        invoiceDetails.Add(detail);
                        if (postDto.PostID != null && postDto.PostID != Guid.Empty)
                            eligiblePostIds.Add(postDto.PostID.Value);
                    }
                }
            }

            invoiceDetails = invoiceDetails
                .OrderBy(x => x.JobDate ?? DateOnly.MaxValue)
                .ThenBy(x => x.JobTime ?? TimeOnly.MaxValue)
                .ThenBy(x => x.OfficerName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            decimal officerFeesTotal = 0m, agencyFeesTotal = 0m;
            if (departmentIdFromToken.Value != Guid.Empty)
            {
                var deptFees = await _departmentFeeService
                    .GetInvoiceEligibleFeesForRequestAsync(departmentIdFromToken.Value, request.RequestID)
                    .ConfigureAwait(false);

                (officerFeesTotal, agencyFeesTotal) = DepartmentFeeInvoiceTotalsHelper.CalculatePerOfficerFeeTotals(
                    deptFees,
                    invoiceDetails,
                    requestCount);
            }

            var billingToContact = PersonNameFormatHelper.FormatLastNameCommaFirstName(request.LastName, request.FirstName)
                ?? billingRequester?.InvoiceContact;

            var departmentInvoiceConfig = (await _departmentInvoiceConfigService.GetAllAsync(null, null, null).ConfigureAwait(false)).FirstOrDefault();
            var payableDirectlyTo = departmentInvoiceConfig?.PayableDirectlyTo;
            var isPayableToOfficer = departmentInvoiceConfig?.IsPayableToOfficer ?? false;
            foreach (var invoiceDetail in invoiceDetails)
            {
                invoiceDetail.PayableDirectlyTo = payableDirectlyTo;
                invoiceDetail.IsPayableToOfficer = isPayableToOfficer;
            }

            var firstPostLength = orderedPosts.FirstOrDefault()?.PostLength;
            var dto = new InvoiceDto(
                requester?.RequesterID,
                billingRequester?.InvoiceBillingName,
                requestAddress,
                billingRequester?.InvoicePhone,
                billingRequester?.InvoiceEmail,
                billingToContact,
                billingFromName,
                billingFromAddress,
                billingFromPhone,
                billingFromEmail,
                billingFromContact)
            {
                InvoiceNo = $"{fiscalYear}{numberStr}",
                InvoiceDetails = invoiceDetails,
                StartDate = startDate,
                EndDate = endDate,
                RequestId = request.RequestID,
                RequestName = requestName,
                PostLength = firstPostLength,
                PayableDirectlyTo = payableDirectlyTo,
                IsPayableToOfficer = isPayableToOfficer,
                DepartmentID = departmentIdFromToken.Value
            };
            dto.ComputeAndSetTotals(officerFeesTotal, agencyFeesTotal);

            if (invoiceDetails.Count == 0)
            {
                var onlyAssignmentClosedWithZeroHours = eligibleAssignmentCount == 1 && closedWithZeroHoursCount == 1;
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    onlyAssignmentClosedWithZeroHours
                        ? ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.EMPLOYEE_CLOSED_JOB_WITH_ZERO_HOURS,
                            requestName,
                            projectNumber,
                            messageStartDate,
                            messageEndDate,
                            string.IsNullOrWhiteSpace(zeroHoursEmployeeName) ? "The employee" : zeroHoursEmployeeName)
                        : ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.NO_EMPLOYEES_JOB_IN_REQUEST,
                            requestName,
                            projectNumber,
                            messageStartDate,
                            messageEndDate));
            }

            return new SuccessResult<InvoiceDto> { Status = (int)HttpStatusCode.OK, Message = string.Empty, Data = dto };
        }

        /// <inheritdoc />
        public async Task<SuccessResult<List<InvoiceDto>>> GetInvoicePreviewDataListAsync(IReadOnlyCollection<string> projectNumbers, DateOnly startDate, DateOnly endDate)
        {
            var results = new List<InvoiceDto>();
            var errors = new List<ErrorDetail>();
            var missingDataErrors = new List<ErrorDetail>();
            var responseStatus = (int)HttpStatusCode.OK;
            foreach (var projectNumber in projectNumbers)
            {
                try
                {
                    var rc = results.Count == 0 ? projectNumbers.Count : 0;
                    var result = await GetRequestDataForInvoiceAsync(projectNumber, startDate, endDate, requestCount: rc).ConfigureAwait(false);
                    if (result.Data != null)
                        results.Add(result.Data);
                }
                catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    missingDataErrors.Add(new ErrorDetail(
                        ex.ErrorMessage,
                        ErrorType.Error,
                        ex.ErrorCode.ToString(CultureInfo.InvariantCulture),
                        projectNumber));
                }
                catch (WebApiException ex)
                {
                    if (responseStatus == (int)HttpStatusCode.OK)
                        responseStatus = ex.ErrorCode;

                    errors.Add(new ErrorDetail(
                        ex.ErrorMessage,
                        ErrorType.Error,
                        ex.ErrorCode.ToString(CultureInfo.InvariantCulture),
                        projectNumber));
                }
            }

            if (results.Count == 0)
            {
                errors.InsertRange(0, missingDataErrors);
                if (responseStatus == (int)HttpStatusCode.OK && missingDataErrors.Count > 0)
                    responseStatus = (int)HttpStatusCode.NotFound;
            }
            else
            {
                // Invoice data is still returned. Jobs that are not closed, requests with no
                // assignments, and closed-with-0-hours stay on the response as errors.
                errors.AddRange(missingDataErrors);
                responseStatus = (int)HttpStatusCode.OK;
            }

            if (results.Count > 1)
                results.Insert(0, CreateCombinedPreviewSummary(results));

            if (errors.Count > 0)
            {
                return new SuccessResult<List<InvoiceDto>>
                {
                    Status = responseStatus,
                    Message = string.Join('\n', errors.Select(error => error.ErrorMessage)),
                    Data = results,
                    Errors = errors
                };
            }

            return new SuccessResult<List<InvoiceDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Message = string.Empty,
                Data = results
            };
        }

        private static string? GetAssignmentEmployeeName(PostPositionDto postPosition, IReadOnlyDictionary<Guid, EmployeeDto> employeeDict)
        {
            if (postPosition.IsPlaceholder)
                return postPosition.PlaceholderDetail?.GetPlaceHolderName();

            return postPosition.EmployeeID != Guid.Empty && employeeDict.TryGetValue(postPosition.EmployeeID, out var employee)
                ? employee.GetFullName()
                : null;
        }

        /// <summary>
        /// Dates shown on a per-request invoice error. Uses that request's own range,
        /// limited to the searched window, so one job is not reported against every selected date.
        /// </summary>
        private static (DateOnly Start, DateOnly End) GetRequestMessageDateRange(Request request, DateOnly searchStart, DateOnly searchEnd)
        {
            var start = request.StartDate > searchStart ? request.StartDate : searchStart;
            var end = request.EndDate < searchEnd ? request.EndDate : searchEnd;
            if (start > end)
                return (request.StartDate, request.EndDate);

            return (start, end);
        }

        private static InvoiceDto CreateCombinedPreviewSummary(IReadOnlyList<InvoiceDto> invoices)
        {
            ArgumentNullException.ThrowIfNull(invoices);
            if (invoices.Count == 0)
                throw new ArgumentException("At least one invoice is required.", nameof(invoices));

            var first = invoices[0];
            var combined = new InvoiceDto(
                first.RequesterID,
                first.BillingToName,
                first.BillingToAddress,
                first.BillingToPhoneNumber,
                first.BillingToEmail,
                first.BillingToContact,
                first.BillingFromName,
                first.BillingFromAddress,
                first.BillingFromPhoneNumber,
                first.BillingFromEmail,
                first.BillingFromContact)
            {
                StartDate = invoices.Min(i => i.StartDate),
                EndDate = invoices.Max(i => i.EndDate),
                DateCreated = first.DateCreated,
                DueDate = first.DueDate,
                DepartmentID = first.DepartmentID,
                InvoiceNo = string.Join(", ", invoices.Select(i => i.InvoiceNo).Where(s => !string.IsNullOrWhiteSpace(s))),
                RequestName = string.Join(", ", invoices.Select(i => i.RequestName).Where(s => !string.IsNullOrWhiteSpace(s))),
                PostLength = first.PostLength,
                InvoiceDetails = invoices
                    .SelectMany(i => i.InvoiceDetails ?? Enumerable.Empty<InvoiceDetailDto>())
                    .OrderBy(x => x.JobDate ?? DateOnly.MaxValue)
                    .ThenBy(x => x.JobTime ?? TimeOnly.MaxValue)
                    .ThenBy(x => x.OfficerName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToList(),
            };

            foreach (var invoice in invoices)
                combined.MergeFrom(invoice, includeLineItems: false);

            return combined;
        }

        private static (string? Name, string? Address, string? Phone, string? Email, string? Contact) GetBillingFromDepartment(DepartmentDto? department)
        {
            if (department == null)
                return (null, null, null, null, null);
            var billingFromContact = PersonNameFormatHelper.FormatLastNameCommaFirstName(department.LastName, department.FirstName)
                ?? department.DepartmentName;
            return (
                department.DepartmentName,
                AddressFormatHelper.BuildSingleLine(department.Address, null, department.City, department.State, department.Zip),
                department.Phone,
                department.Email,
                billingFromContact);
        }

        /// <summary>
        /// Inserts a new request or updates an existing one.
        /// </summary>
        /// <param name="dto">The request DTO to insert or update.</param>
        /// <returns>The upserted request DTO.</returns>
        public override async Task<RequestDto> UpsertAsync(RequestDto dto, bool isSuperUser = false)
        {
            try
            {
                ValidateDtoParameter(dto);

                await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterID.Value, "Requester");

                // Validate SkillIDs if provided
                if (dto.SkillIDs != null && dto.SkillIDs.Any())
                {
                    await _skillService.ValidateSkillIdsAsync(dto.SkillIDs);
                }

                // Fiscal year from invoice config (via service); project number assigned on create.
                // On update, keep existing DB values and backfill only when missing.
                var departmentId = dto.DepartmentID != Guid.Empty
                     ? dto.DepartmentID
                     : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                var fiscalYear = await _departmentInvoiceConfigService
                    .GetFiscalYearForDepartmentAsync(departmentId)
                    .ConfigureAwait(false);

                var existingRequest = dto.RequestID != Guid.Empty
                    ? await _repository.GetByIdAsync(dto.RequestID, "RequestID").ConfigureAwait(false)
                    : null;

                dto.FiscalYear = existingRequest?.FiscalYear ?? fiscalYear;

                var hasProjectNumber = !string.IsNullOrWhiteSpace(existingRequest?.ProjectNumber);
                dto.ProjectNumber = hasProjectNumber
                    ? existingRequest!.ProjectNumber
                    : await _repository.GetNextProjectNumberForYearAsync(dto.FiscalYear ?? fiscalYear).ConfigureAwait(false);

                if (!dto.AlcoholPermitRequired)
                    dto.SpecialRate = 0;

                var isPremiumPaidEventChanged = existingRequest != null
                    && dto.IsPremiumPaidEvent != existingRequest.IsPremiumPaidEvent;
                var futurePostPositionIds = new List<Guid>();
                if (isPremiumPaidEventChanged && existingRequest != null)
                {
                    futurePostPositionIds = await GetFutureAssignmentPostPositionIdsForRequestAsync(existingRequest.RequestID)
                        .ConfigureAwait(false);
                    if (!dto.ConfirmConflict && futurePostPositionIds.Count > 0)
                    {
                        var message = BuildPremiumPayFutureAssignmentsConfirmMessage(dto.IsPremiumPaidEvent);
                        throw new WebApiException(HttpStatusCode.Conflict, ((int)HttpStatusCode.Conflict).ToString(), message);
                    }
                }

                var syncEquipmentEffectiveDates = false;
                if (existingRequest != null)
                {
                    var transferPostPositionIds = dto.TransferWithConflictPostPositionIds?
                        .Where(id => id != Guid.Empty)
                        .Distinct()
                        .ToList();
                    var isTransferringPostPositions = transferPostPositionIds is { Count: > 0 };

                    var requestType = dto.RequestType?.RequestType
                        ?? (await _requestTypeService.GetAllAsync(x => x.RequestID == dto.RequestID).ConfigureAwait(false))
                            .FirstOrDefault()?.RequestType
                        ?? RequestTypes.OneTime;

                    var requestDatesChanged =
                        dto.StartDate != existingRequest.StartDate ||
                        dto.EndDate != existingRequest.EndDate;

                    if (requestType == RequestTypes.OneTime && dto.StartDate != existingRequest.StartDate)
                    {
                        var postPositionService = GetPostPositionServiceFromRequestScope();
                        var oldDate = existingRequest.StartDate;
                        var newDate = dto.StartDate;

                        if (isTransferringPostPositions)
                        {
                            await TransferPostPositionsForOneTimeDateChangeAsync(
                                postPositionService, dto.RequestID, oldDate, newDate, transferPostPositionIds!)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            var reviewData = await postPositionService
                                .GetOneTimeDateChangeReviewDataAsync(dto.RequestID, oldDate, newDate, departmentId)
                                .ConfigureAwait(false);
                            if (reviewData.AssignedOnOldDate.Count > 0)
                                ValidateOneTimeFutureDateChangeAsync(reviewData, newDate);
                        }
                    }

                    if (requestDatesChanged)
                    {
                        // Transfer popup already warns; otherwise require confirm before adjusting equipment dates.
                        if (!dto.ConfirmConflict && !isTransferringPostPositions)
                        {
                            throw new WebApiException(
                                HttpStatusCode.Conflict,
                                ((int)HttpStatusCode.Conflict).ToString(),
                                ModelValidationErrorCodes.GetErrorMessage(
                                    ModelValidationErrorCodes.REQUEST_DATE_CHANGE_EQUIPMENT_DATES_CONFIRM));
                        }

                        syncEquipmentEffectiveDates = true;
                    }
                }

                // Get the upserted request to check if RequiresVehicle changed
                var result = await base.UpsertAsync(dto);

                if (!dto.AlcoholPermitRequired)
                    await ClearPostAlcoholServedOrConsumedFlagsAsync(result.RequestID).ConfigureAwait(false);

                await _postService.SyncPremiumPaidEventFromRequestAsync(result.RequestID, dto.IsPremiumPaidEvent).ConfigureAwait(false);

                if (isPremiumPaidEventChanged && dto.ConfirmConflict)
                {
                    using var scope = _scopeFactory.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<IPostPositionService>()
                        .UpdateFuturePostPositionPayRatesForRequestAsync(result.RequestID)
                        .ConfigureAwait(false);
                }

                if (syncEquipmentEffectiveDates)
                {
                    var effectiveEnd = dto.EndDate < dto.StartDate ? dto.StartDate : dto.EndDate;
                    var editedAt = DateTime.UtcNow;

                    var requestEquipment = (await _requestEquipmentRepository.GetAllAsync(
                            re => re.RequestID == result.RequestID,
                            includes: null,
                            pageParam: null)
                        .ConfigureAwait(false))
                        .ToList();

                    if (requestEquipment.Count > 0)
                    {
                        foreach (var row in requestEquipment)
                        {
                            row.StartDate = dto.StartDate;
                            row.EndDate = effectiveEnd;
                            row.EditedDate = editedAt;
                        }

                        await _requestEquipmentRepository
                            .BulkUpsertAsync(requestEquipment, "RequestEquipmentID")
                            .ConfigureAwait(false);
                    }

                    var requestPosts = (await _postService.GetAllAsync(
                            p => p.RequestID == result.RequestID,
                            includes: null,
                            pageParam: null)
                        .ConfigureAwait(false))
                        .ToList();
                    var postIds = requestPosts
                        .Where(p => p.PostID.HasValue && p.PostID.Value != Guid.Empty)
                        .Select(p => p.PostID!.Value)
                        .Distinct()
                        .ToList();

                    if (postIds.Count > 0)
                    {
                        var postEquipment = (await _postEquipmentRepository.GetAllAsync(
                                pe => postIds.Contains(pe.PostID),
                                includes: null,
                                pageParam: null)
                            .ConfigureAwait(false))
                            .ToList();

                        if (postEquipment.Count > 0)
                        {
                            foreach (var row in postEquipment)
                            {
                                row.StartDate = dto.StartDate;
                                row.EndDate = effectiveEnd;
                                row.EditedDate = editedAt;
                            }

                            await _postEquipmentRepository
                                .BulkUpsertAsync(postEquipment, "PostEquipmentID")
                                .ConfigureAwait(false);
                        }
                    }
                }

                // Delete RequestEquipment records based on RequiresVehicle and RequiresEquipment flags
                await DeleteRequestEquipmentByFlagsAsync(result.RequestID, dto.RequiresVehicle, dto.RequiresEquipment);
                // Post equipment is stored independently per post; request flag changes must not remove it.

                if (existingRequest != null
                    && (HasRequiresFlagChanged(existingRequest.RequiresVehicle, dto.RequiresVehicle)
                        || HasRequiresFlagChanged(existingRequest.RequiresEquipment, dto.RequiresEquipment)))
                {
                    var posts = await _postService.GetAllPostsByRequestIdAsync(result.RequestID).ConfigureAwait(false);
                    foreach (var post in posts)
                    {
                        if (post.PostID is Guid postId && postId != Guid.Empty)
                            await _postService.NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(postId).ConfigureAwait(false);
                    }
                }

                if (dto.RequiresVehicle == false)
                {
                    await RemoveApprovalDataByTypeAsync(result.RequestID, ApprovalType.VehicleApproval);
                }
                if (dto.RequiresPermit == false)
                {
                    await RemoveApprovalDataByTypeAsync(result.RequestID, ApprovalType.PremitApproval);
                }

                // Separate Recurrence (RequestType) from the main DTO to handle it explicitly
                var requestTypeDto = dto.RequestType;
                dto.RequestType = null;

                if (requestTypeDto != null)
                {
                    // Restore it to the result for the response
                    result.RequestType = requestTypeDto;

                    requestTypeDto.RequestID = result.RequestID;

                    var existingRecurrence = (await _requestTypeService.GetAllAsync(x => x.RequestID == result.RequestID)).FirstOrDefault();

                    if (existingRecurrence != null)
                    {
                        requestTypeDto.RequestTypeID = existingRecurrence.RequestTypeID;
                    }

                    // Explicitly Upsert the recurrence / request type
                    result.RequestType = await _requestTypeService.UpsertAsync(requestTypeDto);
                }

                // Handle RequestApprovalRouting if special event approval is required
                if (result.RequestID != Guid.Empty)
                {
                    await HandleSpecialEventApprovalRoutingAsync(dto, result);
                    await HandlePermitApprovalRoutingAsync(dto, result);
                    await SyncRequestDepartmentFeesAsync(result.RequestID, dto.EnableFees, dto.SelectedDepartmentFeeIds)
                        .ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<Guid>> GetFutureAssignmentPostPositionIdsForRequestAsync(Guid requestId)
        {
            var departmentNow = GetCurrentDepartmentDateTime();

            using var scope = _scopeFactory.CreateScope();
            var assignedPositions = await scope.ServiceProvider
                .GetRequiredService<IPostPositionService>()
                .GetAssignedPostPositionsForRequestAsync(requestId)
                .ConfigureAwait(false);

            return assignedPositions
                .Where(pp => pp.PostPositionID.HasValue && !pp.IsPostPositionDateTimePassed(departmentNow))
                .OrderBy(pp => pp.DateOfWork)
                .ThenBy(pp => pp.PostStartTime ?? pp.Post?.PostStartTime)
                .Select(pp => pp.PostPositionID!.Value)
                .ToList();
        }

        private static string BuildPremiumPayFutureAssignmentsConfirmMessage(bool isPremiumPaidEvent) =>
            ModelValidationErrorCodes.GetErrorMessage(
                isPremiumPaidEvent
                    ? ModelValidationErrorCodes.REQUEST_PREMIUM_EVENT_ENABLE_FUTURE_ASSIGNMENT_CONFIRM
                    : ModelValidationErrorCodes.REQUEST_PREMIUM_EVENT_DISABLE_FUTURE_ASSIGNMENT_CONFIRM);

        private async Task HandleSpecialEventApprovalRoutingAsync(RequestDto dto, RequestDto result)
        {
            if (dto.SpecialEventApproved == true)
            {
                var departmentId = result.DepartmentID != Guid.Empty
                    ? result.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                if (departmentId == Guid.Empty)
                {
                    return;
                }

                var specialEventRoutingGroups = (await _routingGroupService.GetAllAsync(
                    x => x.DepartmentID == departmentId && x.ApprovalType == ApprovalType.SpecialEventApproval && x.ApproverStatus == true,
                    null,
                    null)).ToList();

                var specialEventGroup = specialEventRoutingGroups.FirstOrDefault();

                if (specialEventGroup == null)
                {
                    return;
                }

                var specialEventRoutingGroupId = specialEventGroup.RoutingGroupId ?? Guid.Empty;

                var existingRequestApprovalRoutings = (await _requestApprovalRoutingService.GetAllAsync(
                    x => x.RequestID == result.RequestID,
                    null,
                    null)).ToList();

                var existingSpecialEventRouting = existingRequestApprovalRoutings
                    .FirstOrDefault(x => x.ApproverID == specialEventRoutingGroupId);

                if (existingSpecialEventRouting == null)
                {
                    var nextRank = existingRequestApprovalRoutings.Any()
                        ? existingRequestApprovalRoutings.Max(x => x.Rank) + 1
                        : 1;

                    var requestApprovalRouting = new RequestApprovalRoutingDto(
                        specialEventRoutingGroupId,
                        result.RequestID,
                        nextRank);

                    await _requestApprovalRoutingService.UpsertAsync(requestApprovalRouting);
                }

                if (specialEventRoutingGroupId == Guid.Empty)
                {
                    return;
                }

                var baseUrl = _vehicleApprovalLinkBuilder.GetBaseUrl()?.TrimEnd('/') ?? string.Empty;
                var approvalTypeSegment = EnumHelper.ToStringValue(ApprovalType.SpecialEventApproval);
                var systemUrl = string.IsNullOrEmpty(baseUrl)
                    ? $"/{approvalTypeSegment}/{result.RequestID}/{specialEventRoutingGroupId}"
                    : $"{baseUrl}/{approvalTypeSegment}/{result.RequestID}/{specialEventRoutingGroupId}";

                await SendApprovalEmailsAsync(new SendApprovalEmailsRequestDto(
                    result.RequestID,
                    specialEventRoutingGroupId,
                    systemUrl));
            }
            else
            {
                await RemoveApprovalDataByTypeAsync(result.RequestID, ApprovalType.SpecialEventApproval);
            }
        }

        private async Task HandlePermitApprovalRoutingAsync(RequestDto dto, RequestDto result)
        {
            if (dto.RequiresPermit == true)
            {
                var departmentId = result.DepartmentID != Guid.Empty
                    ? result.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                if (departmentId == Guid.Empty)
                {
                    return;
                }

                var permitRoutingGroups = (await _routingGroupService.GetAllAsync(
                    x => x.DepartmentID == departmentId && x.ApprovalType == ApprovalType.PremitApproval && x.ApproverStatus == true,
                    null,
                    null)).ToList();

                var permitGroup = permitRoutingGroups.FirstOrDefault();

                if (permitGroup == null)
                {
                    return;
                }

                var permitRoutingGroupId = permitGroup.RoutingGroupId ?? Guid.Empty;

                var existingRequestApprovalRoutings = (await _requestApprovalRoutingService.GetAllAsync(
                    x => x.RequestID == result.RequestID,
                    null,
                    null)).ToList();

                var existingPermitRouting = existingRequestApprovalRoutings
                    .FirstOrDefault(x => x.ApproverID == permitRoutingGroupId);

                if (existingPermitRouting == null)
                {
                    var nextRank = existingRequestApprovalRoutings.Any()
                        ? existingRequestApprovalRoutings.Max(x => x.Rank) + 1
                        : 1;

                    var requestApprovalRouting = new RequestApprovalRoutingDto(
                        permitRoutingGroupId,
                        result.RequestID,
                        nextRank);

                    await _requestApprovalRoutingService.UpsertAsync(requestApprovalRouting);
                }

                if (permitRoutingGroupId == Guid.Empty)
                {
                    return;
                }

                var baseUrl = _vehicleApprovalLinkBuilder.GetBaseUrl()?.TrimEnd('/') ?? string.Empty;
                var systemUrl = string.IsNullOrEmpty(baseUrl)
                    ? $"/PermitApproval/{result.RequestID}/{permitRoutingGroupId}"
                    : $"{baseUrl}/PermitApproval/{result.RequestID}/{permitRoutingGroupId}";

                await SendApprovalEmailsAsync(new SendApprovalEmailsRequestDto(
                    result.RequestID,
                    permitRoutingGroupId,
                    systemUrl));
            }
            else
            {
                await RemoveApprovalDataByTypeAsync(result.RequestID, ApprovalType.PremitApproval);
            }
        }

        /// <inheritdoc />
        public async Task SendApprovalEmailsAsync(SendApprovalEmailsRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestEntity = await _repository.GetByIdAsync(request.RequestId, "RequestID");
            if (requestEntity == null)
                throw new InvalidOperationException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_ENTITY_NOT_FOUND_WITH_ID, request.RequestId));

            var routingGroup = (await _routingGroupService.GetAllAsync(
                x => x.RoutingGroupId == request.RoutingGroupId,
                null,
                null)).FirstOrDefault();

            if (routingGroup == null)
                throw new InvalidOperationException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ROUTING_GROUP_NOT_FOUND_WITH_ID, request.RoutingGroupId));

            var approverEmail = routingGroup?.ApproverEmail?.Trim();
            if (string.IsNullOrWhiteSpace(approverEmail))
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_APPROVER_EMAIL_NOT_FOUND),
                    nameof(request.RoutingGroupId));

            var approverFullName = routingGroup.ApproverShort?.Trim()
                ?? routingGroup.ApproverLong?.Trim()
                ?? string.Empty;

            var requestName = PersonNameFormatHelper.FormatLastNameCommaFirstName(requestEntity.LastName, requestEntity.FirstName) ?? string.Empty;

            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.FullName)] = approverFullName,
                [EnumHelper.ToStringValue(EmailKeyword.ApprovalLink)] = request.SystemUrl ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.RequestName)] = requestName
            };
            var sendMailRequest = new SendMailRequestDto(
                EmailType.ApprovalEmail,
                approverEmail,
                null,
                templateParameters);

            await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken);
        }

        public async Task SendAssignPostEmailAsync(
            Guid employeeId,
            string? email,
            string postTitle,
            DateOnly? dateOfWork,
            TimeOnly? startTime,
            int? postLength = null,
            string? postLocation = null,
            string? assignedEmployeeName = null,
            string? assignedEmployeeType = null,
            string? assignedEmployeeRank = null,
            string? pushNotificationTitle = null,
            string? pushNotificationBody = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default)
        {
            var resolvedEmail = email?.Trim();
            if (string.IsNullOrWhiteSpace(resolvedEmail))
            {
                var employee = await _employeeService.GetByIdAsync(employeeId, e => e.EmployeeType);
                resolvedEmail = employee?.OffDutyCallBackEmail?.Trim() ?? employee?.PrimaryEmail?.Trim();

                if (string.IsNullOrEmpty(resolvedEmail))
                {
                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                        employee,
                        EmailType.AssignPostEmail,
                        CancellationToken.None).ConfigureAwait(false);
                }

                if (string.IsNullOrEmpty(assignedEmployeeType) && employee != null)
                {
                    assignedEmployeeName = PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? string.Empty;
                    assignedEmployeeType = employee.EmployeeType?.EmployeeTypeLong?.Trim() ?? employee.EmployeeType?.EmployeeTypeShort?.Trim() ?? string.Empty;
                    assignedEmployeeRank = employee.EmployeeType?.Rank > 0
                        ? employee.EmployeeType.Rank.ToString()
                        : string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(resolvedEmail))
                return;

            var postAssignedDate = dateOfWork?.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var postStartTime = startTime?.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;

            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeName)] = assignedEmployeeName ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeType)] = assignedEmployeeType ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeRank)] = assignedEmployeeRank ?? string.Empty
            };

            var sendMailRequest = new SendMailRequestDto(
                EmailType.AssignPostEmail,
                resolvedEmail,
                fromEmail: null,
                templateParameters);

            var departmentTimeZone = await ResolveDepartmentTimeZoneAsync(departmentId, cancellationToken).ConfigureAwait(false);
            sendMailRequest.CalendarInvite = BuildPostAssignmentCalendarInvite(
                postTitle,
                dateOfWork,
                startTime,
                postLength,
                postLocation,
                departmentTimeZone);

            await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken);

            // Best-effort push notification for assigned job
            var title = pushNotificationTitle ?? "Job assigned";
            var body = pushNotificationBody ?? $"{postTitle} on {postAssignedDate} at {postStartTime}";
            await _pushNotificationService.SendToEmployeesAsync(
                new[] { employeeId },
                title,
                body,
                cancellationToken);
        }

        private async Task SendPostTransferEmailAsync(
            string email,
            string postTitle,
            DateOnly dateOfWork,
            TimeOnly startTime,
            TimeOnly endTime,
            string transferFrom,
            string transferTo,
            string vehicles,
            string equipment,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var postAssignedDate = dateOfWork.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            var postStartTime = startTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
            var postEndTime = endTime.ToString("h:mm tt", CultureInfo.InvariantCulture);

            var effectiveDateTime = GetCurrentDepartmentDateTime();
            var transferEffectiveDate = effectiveDateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            var transferEffectiveTime = effectiveDateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
            var transferEffectiveTimestamp = $"{transferEffectiveDate} at {transferEffectiveTime}";

            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.PostEndTime)] = postEndTime,
                [EnumHelper.ToStringValue(EmailKeyword.TransferFrom)] = transferFrom,
                [EnumHelper.ToStringValue(EmailKeyword.TransferTo)] = transferTo,
                [EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveDate)] = transferEffectiveDate,
                [EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveTime)] = transferEffectiveTime,
                [EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveTimestamp)] = transferEffectiveTimestamp,
                // Legacy placeholders kept for existing templates.
                [EnumHelper.ToStringValue(EmailKeyword.SplitBy)] = transferFrom,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = transferTo,
                [EnumHelper.ToStringValue(EmailKeyword.Vehicles)] = vehicles ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.Equipment)] = equipment ?? string.Empty
            };

            var sendMailRequest = new SendMailRequestDto(
                EmailType.PostTransferEmail,
                email.Trim(),
                fromEmail: null,
                templateParameters);

            await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken);
        }

        /// <summary>
        /// Sends transfer-completed notifications. When source and recipient are the same employee, sends one push and one transfer email.
        /// </summary>
        private async Task SendPostTransferNotificationsAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid recipientEmployeeId,
            Guid senderEmployeeId,
            bool isPlaceHolder,
            Guid? assignedPostPositionId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var departmentId = _tokenParser.GetDepartmentID();
                if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                    return;

                var request = await GetRequestByPostId(postId, departmentId.Value);
                var post = request?.Posts?.FirstOrDefault(p => p.PostID == postId);
                if (post == null)
                    return;

                var postTitle = post.PostTitle?.Trim() ?? string.Empty;
                var postAssignedDate = dateOfWork.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                var postStartTime = post.PostStartTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
                var detailLine = $"{postTitle} on {postAssignedDate} at {postStartTime}";

                var vehicles = string.Empty;
                var equipment = string.Empty;
                if (assignedPostPositionId is Guid positionId && positionId != Guid.Empty)
                {
                    using var vehicleScope = _scopeFactory.CreateScope();
                    (vehicles, equipment) = await vehicleScope.ServiceProvider
                        .GetRequiredService<IPostPositionVehicleService>()
                        .GetAssignedDisplayNamesAsync(positionId)
                        .ConfigureAwait(false);
                }

                const string recipientPushTitle = "The post was transferred to you";
                const string senderPushTitle = "Job transferred successfully";

                var isSameEmployee = senderEmployeeId == recipientEmployeeId && senderEmployeeId != Guid.Empty;

                if (isSameEmployee)
                {
                    var employee = await _employeeService.GetByIdAsync(senderEmployeeId, e => e.EmployeeType);
                    var employeeName = employee != null
                        ? PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? string.Empty
                        : string.Empty;

                    await _pushNotificationService.SendToEmployeesAsync(
                        new[] { senderEmployeeId },
                        senderPushTitle,
                        detailLine,
                        cancellationToken);

                    if (!isPlaceHolder)
                    {
                        var email = employee?.OffDutyCallBackEmail?.Trim() ?? employee?.PrimaryEmail?.Trim();

                        if (string.IsNullOrEmpty(email))
                        {
                            await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                                employee,
                                EmailType.PostTransferEmail,
                                cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            await SendPostTransferEmailAsync(
                                email,
                                postTitle,
                                dateOfWork,
                                post.PostStartTime,
                                post.PostEndTime,
                                employeeName,
                                employeeName,
                                vehicles,
                                equipment,
                                cancellationToken);
                        }
                    }

                    return;
                }

                var senderEmployee = await _employeeService.GetByIdAsync(senderEmployeeId, e => e.EmployeeType);
                var recipientEmployee = await _employeeService.GetByIdAsync(recipientEmployeeId, e => e.EmployeeType);

                var senderName = senderEmployee != null
                    ? PersonNameFormatHelper.FormatLastNameCommaFirstName(senderEmployee.LastName, senderEmployee.FirstName) ?? string.Empty
                    : string.Empty;
                var recipientName = recipientEmployee != null
                    ? PersonNameFormatHelper.FormatLastNameCommaFirstName(recipientEmployee.LastName, recipientEmployee.FirstName) ?? string.Empty
                    : string.Empty;

                await _pushNotificationService.SendToEmployeesAsync(
                    new[] { recipientEmployeeId },
                    recipientPushTitle,
                    detailLine,
                    cancellationToken);

                if (!isPlaceHolder && senderEmployeeId != Guid.Empty)
                {
                    await _pushNotificationService.SendToEmployeesAsync(
                        new[] { senderEmployeeId },
                        senderPushTitle,
                        detailLine,
                        cancellationToken);

                    var senderEmail = senderEmployee?.OffDutyCallBackEmail?.Trim() ?? senderEmployee?.PrimaryEmail?.Trim();

                    if (string.IsNullOrEmpty(senderEmail))
                    {
                        await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                            senderEmployee,
                            EmailType.PostTransferEmail,
                            cancellationToken).ConfigureAwait(false);
                    }
                    if (!string.IsNullOrWhiteSpace(senderEmail))
                    {
                        await SendPostTransferEmailAsync(
                            senderEmail,
                            postTitle,
                            dateOfWork,
                            post.PostStartTime,
                            post.PostEndTime,
                            senderName,
                            recipientName,
                            vehicles,
                            equipment,
                            cancellationToken);
                    }
                }
            }
            catch
            {
                // Best-effort: transfer already succeeded.
            }
        }

        private async Task SendPostTransferFailedEmailAsync(
            string email,
            string postTitle,
            DateOnly dateOfWork,
            TimeOnly startTime,
            TimeOnly endTime,
            string transferFrom,
            string transferTo,
            string failureReason,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var postAssignedDate = dateOfWork.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            var postStartTime = startTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
            var postEndTime = endTime.ToString("h:mm tt", CultureInfo.InvariantCulture);

            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.PostEndTime)] = postEndTime,
                [EnumHelper.ToStringValue(EmailKeyword.TransferFrom)] = transferFrom,
                [EnumHelper.ToStringValue(EmailKeyword.TransferTo)] = transferTo,
                [EnumHelper.ToStringValue(EmailKeyword.TransferFailureReason)] = failureReason ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.SplitBy)] = transferFrom,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = transferTo
            };

            var sendMailRequest = new SendMailRequestDto(
                EmailType.PostTransferRolledBackEmail,
                email.Trim(),
                fromEmail: null,
                templateParameters);

            await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken);
        }

        /// <summary>
        /// Notifies the transfer initiator that the transfer did not complete.
        /// </summary>
        private async Task SendPostTransferFailedNotificationToInitiatorAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid? sourceEmployeeId,
            Guid targetEmployeeId,
            string failureReason,
            CancellationToken cancellationToken = default)
        {
            var initiatorEmployeeID = _tokenParser.GetEmployeeID();
            var initiatorEmployee = await _employeeService.GetByIdAsync(initiatorEmployeeID.Value);
            var initiatorEmail = initiatorEmployee?.OffDutyCallBackEmail?.Trim() ?? initiatorEmployee?.PrimaryEmail?.Trim();

            if (string.IsNullOrEmpty(initiatorEmail))
            {
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                    initiatorEmployee,
                    EmailType.PostTransferRolledBackEmail,
                    cancellationToken).ConfigureAwait(false);
                return;
            }

            try
            {
                var post = await _postService.GetByIdAsync(postId).ConfigureAwait(false);
                if (post == null)
                    return;
               
                var sourceEmployee = await _employeeService.GetByIdAsync(sourceEmployeeId.Value);
                string transferFrom = sourceEmployee != null
                    ? PersonNameFormatHelper.FormatLastNameCommaFirstName(sourceEmployee.LastName, sourceEmployee.FirstName) ?? string.Empty
                    : string.Empty;

                var targetEmployee = await _employeeService.GetByIdAsync(targetEmployeeId);
                var transferTo = targetEmployee != null
                    ? PersonNameFormatHelper.FormatLastNameCommaFirstName(targetEmployee.LastName, targetEmployee.FirstName) ?? string.Empty
                    : string.Empty;

                await SendPostTransferFailedEmailAsync(
                    initiatorEmail,
                    post.PostTitle,
                    dateOfWork,
                    post.PostStartTime,
                    post.PostEndTime,
                    transferFrom,
                    transferTo,
                    failureReason,
                    cancellationToken);
            }
            catch
            {
                // Best-effort: transfer failure response must not be blocked by email errors.
            }
        }

        public async Task SendAssignedPostNotificationAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid employeeId,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var resolvedDepartmentId = departmentId ?? _tokenParser.GetDepartmentID();
                if (!resolvedDepartmentId.HasValue || resolvedDepartmentId.Value == Guid.Empty)
                {
                    _logger.LogWarning(
                        "Skipping assigned-job notification because department is missing. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}",
                        postId,
                        dateOfWork,
                        employeeId);
                    return;
                }

                var request = await GetRequestByPostId(postId, resolvedDepartmentId.Value);
                var post = request?.Posts?.FirstOrDefault(p => p.PostID == postId);
                var assignedEmployee = await _employeeService.GetByIdAsync(employeeId, e => e.EmployeeType);
                var recipientEmail = assignedEmployee?.OffDutyCallBackEmail?.Trim() ?? assignedEmployee?.PrimaryEmail?.Trim();

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    _logger.LogWarning(
                        "Assigned-job email is unavailable for EmployeeId: {EmployeeId}. Sending fallback email-unavailable notification.",
                        employeeId);
                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                        assignedEmployee,
                        EmailType.AssignPostEmail,
                        cancellationToken).ConfigureAwait(false);
                }

                if (string.IsNullOrWhiteSpace(recipientEmail) || post == null)
                {
                    _logger.LogWarning(
                        "Skipping assigned-job notification. RecipientEmailMissing: {RecipientEmailMissing}, PostFound: {PostFound}, PostId: {PostId}, EmployeeId: {EmployeeId}, DateOfWork: {DateOfWork}",
                        string.IsNullOrWhiteSpace(recipientEmail),
                        post != null,
                        postId,
                        employeeId,
                        dateOfWork);
                    return;
                }

                var assignedEmployeeName = string.Empty;
                var assignedEmployeeRank = string.Empty;
                var assignedEmployeeType = string.Empty;

                if (assignedEmployee != null)
                {
                    assignedEmployeeName = PersonNameFormatHelper.FormatLastNameCommaFirstName(assignedEmployee.LastName, assignedEmployee.FirstName) ?? string.Empty;
                    var rank = assignedEmployee.EmployeeType?.Rank;
                    assignedEmployeeRank = (rank.HasValue && rank.Value != 0) ? rank.Value.ToString() : string.Empty;
                    assignedEmployeeType = assignedEmployee.EmployeeType?.EmployeeTypeLong?.Trim()
                        ?? assignedEmployee.EmployeeType?.EmployeeTypeShort?.Trim()
                        ?? string.Empty;
                }

                await SendAssignPostEmailAsync(
                    employeeId,
                    recipientEmail,
                    post.PostTitle?.Trim() ?? string.Empty,
                    dateOfWork,
                    post.PostStartTime,
                    post.PostLength,
                    post.PostLocation,
                    assignedEmployeeName,
                    assignedEmployeeType,
                    assignedEmployeeRank,
                    pushNotificationTitle: null,
                    pushNotificationBody: null,
                    departmentId: resolvedDepartmentId,
                    cancellationToken: cancellationToken);
                _logger.LogInformation(
                    "Assigned-job notification sent. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}, Email: {Email}",
                    postId,
                    dateOfWork,
                    employeeId,
                    recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Assigned-job notification failed. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}",
                    postId,
                    dateOfWork,
                    employeeId);
                // Best-effort: assignment/transfer already succeeded.
            }
        }

        private static EmailCalendarInviteDto? BuildPostAssignmentCalendarInvite(
            string postTitle,
            DateOnly? dateOfWork,
            TimeOnly? startTime,
            int? postLength,
            string? postLocation,
            TimeZoneInfo timeZone)
        {
            if (!dateOfWork.HasValue || !startTime.HasValue)
                return null;

            var localStart = dateOfWork.Value.ToDateTime(startTime.Value);
            var localEnd = localStart.AddMinutes(postLength.GetValueOrDefault() > 0 ? postLength.Value : 60);

            var unspecifiedStart = DateTime.SpecifyKind(localStart, DateTimeKind.Unspecified);
            var unspecifiedEnd = DateTime.SpecifyKind(localEnd, DateTimeKind.Unspecified);

            return new EmailCalendarInviteDto
            {
                Summary = postTitle ?? "Assigned job",
                Description = $"Assigned job on {dateOfWork.Value.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)} at {startTime.Value.ToString("h:mm tt", CultureInfo.InvariantCulture)}.",
                Location = postLocation,
                StartUtc = TimeZoneInfo.ConvertTimeToUtc(unspecifiedStart, timeZone),
                EndUtc = TimeZoneInfo.ConvertTimeToUtc(unspecifiedEnd, timeZone),
                FileName = "post-assignment.ics"
            };
        }

        private async Task<TimeZoneInfo> ResolveDepartmentTimeZoneAsync(
            Guid? departmentId = null,
            CancellationToken cancellationToken = default)
        {
            var timeZoneId = _tokenParser.GetTimeZoneId();
            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                var resolvedDepartmentId = departmentId ?? _tokenParser.GetDepartmentID();
                if (resolvedDepartmentId.HasValue && resolvedDepartmentId.Value != Guid.Empty)
                {
                    var department = await _departmentService.GetByIdAsync(resolvedDepartmentId.Value).ConfigureAwait(false);
                    timeZoneId = department?.TimeZoneId;
                }
            }

            return timeZoneId.ToSafeTimeZone();
        }

        /// <summary>
        /// Notifies the employee who held a cancelled or cancellation-pending slot that another employee has taken that slot.
        /// Best-effort only; failures are ignored so assignment flow is not blocked.
        /// </summary>
        private async Task SendCancelledJobRefilledEmailAsync(
            Guid priorSlotHolderEmployeeId,
            Guid assignedEmployeeId,
            Guid postId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken)
        {
            try
            {
                var departmentId = _tokenParser.GetDepartmentID();
                if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                    return;

                var request = await GetRequestByPostId(postId, departmentId.Value).ConfigureAwait(false);
                var post = request?.Posts?.FirstOrDefault(p => p.PostID == postId);
                if (post == null)
                    return;

                var canceller = await _employeeService.GetByIdAsync(priorSlotHolderEmployeeId, e => e.EmployeeType).ConfigureAwait(false);
                var cancellerEmail = canceller?.OffDutyCallBackEmail?.Trim() ?? canceller?.PrimaryEmail?.Trim();
                if (string.IsNullOrEmpty(cancellerEmail))
                {
                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                        canceller,
                        EmailType.CancelledJobRefilledEmail,
                        cancellationToken).ConfigureAwait(false);
                    return;
                }

                var filler = await _employeeService.GetByIdAsync(assignedEmployeeId, e => e.EmployeeType).ConfigureAwait(false);
                var fillerName = filler != null
                    ? PersonNameFormatHelper.FormatLastNameCommaFirstName(filler.LastName, filler.FirstName) ?? string.Empty
                    : string.Empty;
                var fillerRank = filler?.EmployeeType?.Rank;
                var fillerRankStr = (fillerRank.HasValue && fillerRank.Value != 0) ? fillerRank.Value.ToString() : string.Empty;
                var fillerType = filler?.EmployeeType?.EmployeeTypeLong?.Trim()
                    ?? filler?.EmployeeType?.EmployeeTypeShort?.Trim()
                    ?? string.Empty;

                var postTitle = post.PostTitle?.Trim() ?? string.Empty;
                var postAssignedDate = dateOfWork.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                var postStartTime = post.PostStartTime.ToString("h:mm tt", CultureInfo.InvariantCulture);

                var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                    [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                    [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                    [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeName)] = fillerName,
                    [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeType)] = fillerType,
                    [EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeRank)] = fillerRankStr
                };

                var sendMailRequest = new SendMailRequestDto(
                    EmailType.CancelledJobRefilledEmail,
                    cancellerEmail,
                    fromEmail: null,
                    templateParameters);

                await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Best-effort: assignment already succeeded.
            }
        }

        public async Task<SuccessResult<PostPositionDto>> AssignPostToMe(Guid postId, DateOnly dateOfWork, Guid? employeeId = null, bool confirmConflict = false, bool isBulkAssign = false, Guid? postPositionId = null, bool isPlaceHolder = false, PlaceholderDto? placeholder = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null)
        {
            SuccessResult<PostPositionDto> postPosition = new SuccessResult<PostPositionDto>();
            if (!isPlaceHolder)
            {
                // Bulk assign is already authorized at the controller; skip EmployeesViewManage override checks.
                var (subjectEmployeeId, _) = await EmployeeJobsSubjectHelper.ResolveSubjectEmployeeAsync(
                    employeeId,
                    _tokenParser,
                    _employeeService,
                    skipPrivilegeCheck: isBulkAssign).ConfigureAwait(false);
                employeeId = subjectEmployeeId == Guid.Empty ? null : subjectEmployeeId;
            }
            else
            {
                employeeId ??= _tokenParser.GetEmployeeID();
            }

            var placeholderDisplayName = placeholder?.GetPlaceHolderName();
            _logger.LogInformation(
                "AssignPostToMe requested. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}, ConfirmConflict: {ConfirmConflict}, IsBulkAssign: {IsBulkAssign}, PostPositionId: {PostPositionId}, IsPlaceholder: {IsPlaceholder}",
                postId,
                dateOfWork,
                employeeId,
                confirmConflict,
                isBulkAssign,
                postPositionId,
                isPlaceHolder);

            if (!employeeId.HasValue || employeeId == Guid.Empty && !isPlaceHolder)
                throw new WebApiException(
                    HttpStatusCode.Unauthorized,
                    "401",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_EMPLOYEE_ID_MISSING));

            Guid? priorCancelledSlotHolderEmployeeId = null;
            PostPositionStatus? priorSlotStatus = null;
            string? priorSlotEmployeeName = null;
            Guid? rollbackToCancellationPendingPostPositionId = null;
            if (postPositionId.HasValue && postPositionId.Value != Guid.Empty)
            {
                var slotForRefill = await _postPositionRepository
                    .GetByIdAsync(postPositionId.Value, "PostPositionID", x => x.Employee!)
                    .ConfigureAwait(false);
                if (slotForRefill != null &&
                    slotForRefill.PostID == postId &&
                    slotForRefill.DateOfWork == dateOfWork)
                {
                    if (slotForRefill.PostPositionStatus == PostPositionStatus.CancellationPending)
                    {
                        await _postPositionRepository
                            .UpdatePostPositionStatusDirectAsync(
                                postPositionId.Value,
                                PostPositionStatus.Cancelled)
                            .ConfigureAwait(false);
                        rollbackToCancellationPendingPostPositionId = postPositionId.Value;
                    }

                    if (slotForRefill.EmployeeID != Guid.Empty &&
                        slotForRefill.EmployeeID != employeeId.Value &&
                        (slotForRefill.PostPositionStatus == PostPositionStatus.CancellationPending ||
                         slotForRefill.PostPositionStatus == PostPositionStatus.Cancelled ||
                         slotForRefill.PostPositionStatus == PostPositionStatus.Transferred))
                    {
                        priorCancelledSlotHolderEmployeeId = slotForRefill.EmployeeID;
                        priorSlotStatus = slotForRefill.PostPositionStatus;
                        if (slotForRefill.Employee != null)
                            priorSlotEmployeeName = BaseDto.FormatDisplayName(slotForRefill.Employee.LastName, slotForRefill.Employee.FirstName);
                    }
                }
            }

            var assignCompleted = false;
            try
            {
                var result = await CheckAvailablePost(postId, dateOfWork, employeeId, isBulkAssign, postPositionId, isPlaceHolder, placeholderDisplayName).ConfigureAwait(false);
                var auditContext = new PostWorkflowAuditContext
                {
                    PostId = postId,
                    EmployeeId = employeeId,
                    FailureReason = result.Message
                };

                if (result.HasTimeConflictRequiringConfirmation && !confirmConflict)
                {
                    // Return conflict status code (409) so frontend can display confirmation dialog
                    // The frontend will call this endpoint again with confirmConflict=true after user confirmation
                    postPosition.Status = (int)HttpStatusCode.Conflict;
                    postPosition.Message = result.Message;
                    _logger.LogWarning(
                        "AssignPostToMe blocked by time conflict confirmation. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}, Reason: {Reason}",
                        postId,
                        dateOfWork,
                        employeeId,
                        result.Message);
                    await _postWorkflowAuditService.LogFailureAsync(PostPositionStatus.Assigned, context: auditContext).ConfigureAwait(false);
                }
                else if (result.HasCancellationPendingSlotsRequiringDialog)
                {
                    postPosition.Status = (int)HttpStatusCode.UnprocessableEntity;
                    postPosition.Message = result.Message;
                    _logger.LogWarning(
                        "AssignPostToMe requires cancellation-pending confirmation. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}, Reason: {Reason}",
                        postId,
                        dateOfWork,
                        employeeId,
                        result.Message);
                }
                // Bulk/single request may send confirmConflict=true for a prior employee; only a *time* conflict
                // may be overridden by that flag—other errors (e.g. no slots) must still return 400.
                else if (result.HasError && !result.HasTimeConflictRequiringConfirmation)
                {
                    postPosition.Status = (int)HttpStatusCode.BadRequest;
                    postPosition.Message = result.Message;
                    _logger.LogWarning(
                        "AssignPostToMe failed. PostId: {PostId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}, Reason: {Reason}",
                        postId,
                        dateOfWork,
                        employeeId,
                        result.Message);
                    await _postWorkflowAuditService.LogFailureAsync(PostPositionStatus.Assigned, context: auditContext).ConfigureAwait(false);
                }
                else
                {
                    // Proceed: availability OK, or time overlap was acknowledged via confirmConflict
                    postPosition.Status = (int)HttpStatusCode.OK;
                    postPosition.Data = await _postService.AssignToPostMeAsync(postId, employeeId.Value, dateOfWork, isPlaceHolder, placeholder, postPositionId, enableInvoiceAndPayroll, waitingPostPositionPayRate);
                    postPosition.Message = $"{postPosition.Data.Post.PostTitle}: job assigned successfully";
                    assignCompleted = true;
                    _logger.LogInformation(
                        "AssignPostToMe succeeded. PostId: {PostId}, PostPositionId: {PostPositionId}, DateOfWork: {DateOfWork}, EmployeeId: {EmployeeId}",
                        postId,
                        postPosition.Data?.PostPositionID,
                        dateOfWork,
                        employeeId);
                    await _postWorkflowAuditService.LogSuccessAsync(
                        PostPositionStatus.Assigned,
                        new PostWorkflowAuditContext
                        {
                            PostId = postId,
                            PostPositionId = postPosition.Data?.PostPositionID,
                            EmployeeId = employeeId
                        }).ConfigureAwait(false);

                    if (postPosition.Data?.PostPositionID is { } createdPositionId && createdPositionId != Guid.Empty)
                    {
                        var performedByEmployeeId = _tokenParser.GetEmployeeID();
                        var isTransferAssign = priorSlotStatus == PostPositionStatus.Transferred
                            && priorCancelledSlotHolderEmployeeId.HasValue;
                        var isSelfTake = !isBulkAssign
                            && !isTransferAssign
                            && performedByEmployeeId.HasValue
                            && performedByEmployeeId.Value == employeeId.Value;

                        if (isTransferAssign)
                        {
                            await _postPositionFlowAuditService.LogAsync(
                                PostPositionFlowEventType.Transferred,
                                new PostPositionFlowAuditContext
                                {
                                    PostPositionId = createdPositionId,
                                    RelatedPostPositionId = postPositionId,
                                    PostId = postId,
                                    EmployeeId = employeeId,
                                    RelatedEmployeeId = priorCancelledSlotHolderEmployeeId,
                                    RelatedEmployeeName = priorSlotEmployeeName,
                                    PerformedByEmployeeId = performedByEmployeeId
                                }).ConfigureAwait(false);
                        }
                        else
                        {
                            await _postPositionFlowAuditService.LogAsync(
                                isSelfTake
                                    ? PostPositionFlowEventType.SelfTaken
                                    : PostPositionFlowEventType.AssignedByAdmin,
                                new PostPositionFlowAuditContext
                                {
                                    PostPositionId = createdPositionId,
                                    RelatedPostPositionId = postPositionId,
                                    PostId = postId,
                                    EmployeeId = employeeId,
                                    PerformedByEmployeeId = performedByEmployeeId
                                }).ConfigureAwait(false);
                        }
                    }

                    if (priorCancelledSlotHolderEmployeeId.HasValue &&
                        priorCancelledSlotHolderEmployeeId.Value != Guid.Empty &&
                        priorCancelledSlotHolderEmployeeId.Value != employeeId.Value)
                    {
                        await SendCancelledJobRefilledEmailAsync(
                            priorCancelledSlotHolderEmployeeId.Value,
                            employeeId.Value,
                            postId,
                            dateOfWork,
                            CancellationToken.None).ConfigureAwait(false);
                    }
                    if(!isPlaceHolder)
                    {
                        var departmentId = _tokenParser.GetDepartmentID();
                        await SendAssignedPostNotificationAsync(postId, dateOfWork, employeeId.Value, departmentId);
                    }
                }

                return postPosition;
            }
            finally
            {
                if (rollbackToCancellationPendingPostPositionId.HasValue && !assignCompleted)
                {
                    await _postPositionRepository
                        .UpdatePostPositionStatusDirectAsync(
                            rollbackToCancellationPendingPostPositionId.Value,
                            PostPositionStatus.CancellationPending)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<AvailablePostResultDto> CheckAvailablePost(Guid postId, DateOnly dateOfWork, Guid? employeeId = null, bool isBulkAssign = false, Guid? postPositionId = null,bool isPlaceHolder =false,string placeHolder =null)
        {
            using var scope = _scopeFactory.CreateScope();
            var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();
            var waitingPostPositionService = scope.ServiceProvider.GetRequiredService<IWaitingPostPositionService>();
            var requesterRestrictedListService = scope.ServiceProvider.GetRequiredService<IRequesterRestrictedListService>();
            Guid? departmentId = _tokenParser.GetDepartmentID();
            if (departmentId == null)
                return new AvailablePostResultDto();

            var request = await GetRequestByPostId(postId, departmentId.Value);
            //filtering extra posts using the postid
            if (request == null)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    "404",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_NOT_FOUND_GENERIC));
            request.TrimPostByPostId(postId);

            EmployeeDto? employee = null;
            List<PostPositionDto>? employeeCurrentPostPositions = null;
            List<WaitingPostPositionDto>? employeeWaitingPostposition = null;
            List<RequesterRestrictedListDto>? restrictedEmployeeList = null;
            EmployeePolicyUsageDto? employeeStatistics = null;

            var (searchDate, effectiveSearchDateTime) = GetDateAsPerDepartmentTimeZone(dateOfWork);
            var departmentpolicy = _tokenParser.GetDepartmentPolicy();
            var isDepartmentAdmin = _tokenParser.IsDepartmentAdmin();

            if (!isPlaceHolder)
            {
                Expression<Func<Employee, object>>[] includes = { e => e.EmployeeType, e => e.EmployeeSkills };
                employee = await _employeeService.GetByIdAsync(employeeId.Value, includes);
                restrictedEmployeeList = await requesterRestrictedListService.GetRequesterIdsByEmployeeId(employeeId.Value, searchDate, searchDate);
                employeeCurrentPostPositions = await postPositionService.GetPostPositionByEmployeeAndDateRangeAsync(employeeId.Value, searchDate, searchDate, departmentId);
                employeeStatistics = await postPositionService.GetEmployeeStatistics(departmentpolicy, employee.EmployeeID.Value, searchDate, effectiveSearchDateTime);
                employeeWaitingPostposition = await waitingPostPositionService.GetWaitingPostPositionsByEmployeeAndDateRangeAsync(employeeId.Value, searchDate, searchDate, departmentId);
            }
            var releaseAs = await _repository.GetLatestReleaseAsForRequestAsync(request.RequestID).ConfigureAwait(false);
            var availablePost = request.GetAvailablePosts(employee, effectiveSearchDateTime, effectiveSearchDateTime, dateOfWork, releaseAs, employeeCurrentPostPositions, null, restrictedEmployeeList, departmentpolicy, employeeStatistics, false, isDepartmentAdmin, validatePost: true, postPositionId, isBulkAssign, browseOnlyTimeConflict: false, isPlaceHolder,placeHolder);

            if (!availablePost.HasCancellationPendingSlotsRequiringDialog
                && postPositionId.HasValue
                && postPositionId != Guid.Empty)
            {
                var sourceSlot = await postPositionService.GetByIdAsync(postPositionId.Value).ConfigureAwait(false);
                if (sourceSlot?.PostPositionStatus == PostPositionStatus.CancellationPending)
                {
                    await postPositionService.UpdateStatusByPostIdAsync(postPositionId.Value, PostPositionStatus.Cancelled);
                }
            }

            return availablePost;
        }

        public async Task<AvailablePostResultDto> CheckAvailablePostForJobAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid employeeId,
            Guid departmentId,
            string timeZoneId,
            DepartmentPolicyDto? departmentPolicy,
            bool isBulkAssign = false)
        {
            using var scope = _scopeFactory.CreateScope();
            var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            using var _ = new JobHttpContextScope(
                httpContextAccessor,
                departmentId: departmentId,
                employeeId: employeeId,
                timeZoneId: timeZoneId,
                departmentPolicy: departmentPolicy);

            return await CheckAvailablePost(postId, dateOfWork, employeeId, isBulkAssign).ConfigureAwait(false);
        }

        public async Task<SuccessResult<List<BulkAssignPostPositionItemDto>>> BulkUpsertPostPositionsAsync(BulkAssignPostPositionRequestDto bulkAssignPostPositionRequestDto)
        {
            if (bulkAssignPostPositionRequestDto.IsTransfer)
            {
                if (bulkAssignPostPositionRequestDto.IsPlaceholder)
                {
                    await _placeholderService.ValidateBulkEmployeeNumbersAsync(bulkAssignPostPositionRequestDto.Placeholders).ConfigureAwait(false);
                    await _placeholderService.CreateMissingAndSetIdsAsync(bulkAssignPostPositionRequestDto.Placeholders).ConfigureAwait(false);
                }

                return await TransferPostPositionsAsync(bulkAssignPostPositionRequestDto).ConfigureAwait(false);
            }

            if (bulkAssignPostPositionRequestDto.IsPlaceholder)
                await _placeholderService.ValidateBulkEmployeeNumbersAsync(bulkAssignPostPositionRequestDto.Placeholders).ConfigureAwait(false);

            var postWithRequest = await _postService.GetPostWithRequestAsync(bulkAssignPostPositionRequestDto.PostID).ConfigureAwait(false);
            if (postWithRequest?.Request?.IsSpecialEvent == true
                && postWithRequest.RequestID.HasValue
                && !bulkAssignPostPositionRequestDto.IsPlaceholder)
            {
                return await BulkUpsertSpecialEventPostPositionsAsync(
                    bulkAssignPostPositionRequestDto,
                    postWithRequest.RequestID.Value).ConfigureAwait(false);
            }

            var bulkAssignedPostPosition = CreateBulkAssignResult();
            var successfulEmployeeIds = new List<Guid>();

            // Resolve latest cancelled slot whose CallSign will be used only after normal open slots are consumed.
            var sourcePostPositionIdForCallSign = await GetPostPositionServiceFromRequestScope()
                .CallSignBulkAssignSourcePostPositionIdAsync(bulkAssignPostPositionRequestDto.PostID, bulkAssignPostPositionRequestDto.DateOfWork)
                .ConfigureAwait(false);

            var regularBookedStatuses = new[]
            {
                PostPositionStatus.Assigned,
                PostPositionStatus.SplitDecisionPending,
                PostPositionStatus.CancellationPending,
                PostPositionStatus.Unclosed,
                PostPositionStatus.Closed
            };

            var regularBookedCount = (await _postPositionRepository.GetAllAsync(
                filter: x =>
                    x.PostID == bulkAssignPostPositionRequestDto.PostID &&
                    x.DateOfWork == bulkAssignPostPositionRequestDto.DateOfWork &&
                    regularBookedStatuses.Contains(x.PostPositionStatus),
                includes: null,
                pageParam: null).ConfigureAwait(false)).Count();

            var cancelledCount = (await _postPositionRepository.GetAllAsync(
                filter: x =>
                    x.PostID == bulkAssignPostPositionRequestDto.PostID &&
                    x.DateOfWork == bulkAssignPostPositionRequestDto.DateOfWork &&
                    x.PostPositionStatus == PostPositionStatus.Cancelled,
                includes: null,
                pageParam: null).ConfigureAwait(false)).Count();

            // "Regular open" slots are only those not already represented by active bookings
            // and not represented by cancelled rows waiting for refill.
            var regularSlotsToFillFirst = Math.Max(0, postWithRequest.TotalPostPositions - regularBookedCount - cancelledCount);

            var sourceIdForThisAssign = regularSlotsToFillFirst > 0 ? null : sourcePostPositionIdForCallSign;

            if (bulkAssignPostPositionRequestDto.IsPlaceholder)
            {
                foreach (var item in bulkAssignPostPositionRequestDto.Placeholders ?? [])
                {
                    var hasPlaceholderId = item.PlaceholderID.HasValue && item.PlaceholderID.Value != Guid.Empty;
                    var hasName = !string.IsNullOrWhiteSpace(item.FirstName) && !string.IsNullOrWhiteSpace(item.LastName);
                    if (!hasPlaceholderId && !hasName)
                        continue;

                    var postPosition = await AssignPostToMe(
                        bulkAssignPostPositionRequestDto.PostID,
                        bulkAssignPostPositionRequestDto.DateOfWork,
                        Guid.Empty,
                        bulkAssignPostPositionRequestDto.ConfirmConflict,
                        isBulkAssign: true,
                        postPositionId: sourceIdForThisAssign,
                        isPlaceHolder: true,
                        placeholder: _mapper.Map<PlaceholderDto>(item),
                        enableInvoiceAndPayroll: item.EnableInvoiceAndPayroll).ConfigureAwait(false);

                    if (postPosition.Status != 200)
                    {
                        var timeConflict = postPosition.Status == 409;
                        AddBulkAssignErrorItem(
                            bulkAssignedPostPosition,
                            bulkAssignPostPositionRequestDto.PostID,
                            bulkAssignPostPositionRequestDto.DateOfWork,
                            postPosition.Message,
                            hasTimeConflictRequiringConfirmation: timeConflict);
                        if (timeConflict)
                            break;
                    }
                    else if (postPosition.Data != null)
                    {
                        AddBulkAssignSuccessItem(bulkAssignedPostPosition, postPosition.Data);
                        if (regularSlotsToFillFirst > 0)
                            regularSlotsToFillFirst--;
                    }
                }
            }
            else
            {
                foreach (var employeeId in bulkAssignPostPositionRequestDto.EmployeeIDs.Where(id => id != Guid.Empty))
                {
                    var postPosition = await AssignPostToMe(
                        bulkAssignPostPositionRequestDto.PostID,
                        bulkAssignPostPositionRequestDto.DateOfWork,
                        employeeId,
                        bulkAssignPostPositionRequestDto.ConfirmConflict,
                        isBulkAssign: true,
                        postPositionId: sourceIdForThisAssign,
                        enableInvoiceAndPayroll: false).ConfigureAwait(false);

                    if (postPosition.Status != 200)
                    {
                        var timeConflict = postPosition.Status == 409;
                        AddBulkAssignErrorItem(
                            bulkAssignedPostPosition,
                            bulkAssignPostPositionRequestDto.PostID,
                            bulkAssignPostPositionRequestDto.DateOfWork,
                            postPosition.Message,
                            hasTimeConflictRequiringConfirmation: timeConflict);
                        if (timeConflict)
                            break;
                    }
                    else if (postPosition.Data != null)
                    {
                        AddBulkAssignSuccessItem(bulkAssignedPostPosition, postPosition.Data);
                        successfulEmployeeIds.Add(employeeId);
                        if (regularSlotsToFillFirst > 0)
                            regularSlotsToFillFirst--;
                    }
                }
            }

            if (successfulEmployeeIds.Count > 0 && !bulkAssignPostPositionRequestDto.IsPlaceholder)
            {
                await _pushNotificationService.SendToEmployeesAsync(
                    successfulEmployeeIds,
                    "Job assigned",
                    $"You have been assigned to a job (ID {bulkAssignPostPositionRequestDto.PostID}).");
            }

            return bulkAssignedPostPosition;
        }

        private IPostPositionService GetPostPositionServiceFromRequestScope()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new WebApiException(
                    HttpStatusCode.Unauthorized,
                    ((int)HttpStatusCode.Unauthorized).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_CONTEXT_NOT_AVAILABLE));

            return httpContext.RequestServices.GetRequiredService<IPostPositionService>();
        }

        private async Task<SuccessResult<List<BulkAssignPostPositionItemDto>>> BulkUpsertSpecialEventPostPositionsAsync(
            BulkAssignPostPositionRequestDto requestDto,
            Guid requestId)
        {
            var result = CreateBulkAssignResult();
            var successfulEmployeeIds = new List<Guid>();
            using var scope = _scopeFactory.CreateScope();
            var waitingPostPositionService = scope.ServiceProvider.GetRequiredService<IWaitingPostPositionService>();

            var pendingRowsByEmployee = await waitingPostPositionService
                .GetPendingSpecialEventRowsByEmployeeAsync(requestId, requestDto.DateOfWork)
                .ConfigureAwait(false);

            foreach (var employeeId in requestDto.EmployeeIDs.Where(id => id != Guid.Empty))
            {
                var hasPendingRow = pendingRowsByEmployee.TryGetValue(employeeId, out var waitingRow);

                var assignResult = await AssignPostToMe(
                    requestDto.PostID,
                    requestDto.DateOfWork,
                    employeeId,
                    requestDto.ConfirmConflict,
                    isBulkAssign: true,
                    waitingPostPositionPayRate: hasPendingRow ? waitingRow?.WaitingPostPositionPayRate : null).ConfigureAwait(false);

                if (assignResult.Status != 200 || assignResult.Data == null)
                {
                    var failMessage = string.IsNullOrWhiteSpace(assignResult.Message)
                        ? "Employee is not eligible for assignment on this date."
                        : assignResult.Message;
                    var timeConflict = assignResult.Status == 409;

                    if (hasPendingRow && waitingRow != null && waitingRow.WaitingPostPositionID is Guid wppIdFail)
                    {
                        await waitingPostPositionService.MarkSpecialEventAssignmentFailedAsync(wppIdFail,failMessage,assignResult.Status).ConfigureAwait(false);
                    }
                    AddBulkAssignErrorItem(result,requestDto.PostID,requestDto.DateOfWork,failMessage);
                    if (timeConflict)
                        break;
                    continue;
                }

                if (hasPendingRow && waitingRow != null && waitingRow.WaitingPostPositionID is Guid wppIdSuccess)
                {
                    await waitingPostPositionService.MarkWaitingRowAssignedAsync(wppIdSuccess, requestDto.PostID).ConfigureAwait(false);
                }

                AddBulkAssignSuccessItem(result, assignResult.Data);
                successfulEmployeeIds.Add(employeeId);
            }

            if (successfulEmployeeIds.Count > 0)
            {
                await _pushNotificationService.SendToEmployeesAsync(
                    successfulEmployeeIds,
                    "Post assigned",
                    $"You have been assigned to a post for {requestDto.PostID}.").ConfigureAwait(false);
            }

            return result;
        }

        private async Task<SuccessResult<List<BulkAssignPostPositionItemDto>>> TransferPostPositionsAsync(
            BulkAssignPostPositionRequestDto req)
        {
            var result = CreateBulkAssignResult();
            var placeholderItem = req.IsPlaceholder ? req.Placeholders?.FirstOrDefault() : null;
            var targetEmployeeId = req.IsPlaceholder
                ? placeholderItem!.PlaceholderID!.Value
                : req.EmployeeIDs[0];
            var actionEmployeeId = _tokenParser.GetEmployeeID();
            var actionUsername = _tokenParser.GetUsername();

            PostPositionDto? source = null;
            var newlyPersistedPositionIds = new List<Guid>();
            try
            {
                var postPositionService = GetPostPositionServiceFromRequestScope();
                source = await postPositionService.ValidateTransferSourceAsync(
                    req.SourcePostPositionId!.Value,
                    req.PostID,
                    req.DateOfWork,
                    targetEmployeeId).ConfigureAwait(false);

                using var vehicleScope = _scopeFactory.CreateScope();
                var vehicleService = vehicleScope.ServiceProvider.GetRequiredService<IPostPositionVehicleService>();

                var filledIds = (await postPositionService.GetAllAsync(
                    pp => pp.PostID == req.PostID
                          && pp.DateOfWork == req.DateOfWork
                          && !pp.IsPlaceholder
                          && pp.EmployeeID != Guid.Empty
                          && (pp.PostPositionStatus == PostPositionStatus.Assigned
                              || pp.PostPositionStatus == PostPositionStatus.CancellationPending
                              || pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending
                              || pp.PostPositionStatus == PostPositionStatus.Unclosed
                              || pp.PostPositionStatus == PostPositionStatus.Closed),
                    null,
                    null).ConfigureAwait(false))
                    .Select(p => p.PostPositionID.GetValueOrDefault())
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (!filledIds.Contains(req.SourcePostPositionId!.Value))
                    filledIds.Add(req.SourcePostPositionId!.Value);

                var alreadyStored = await vehicleService.GetAllocatedPositionIdsAsync(filledIds).ConfigureAwait(false);
                foreach (var filledId in filledIds)
                    await vehicleService.CopyAllocationsAsync(filledId, filledId).ConfigureAwait(false);
                newlyPersistedPositionIds.AddRange(filledIds.Where(id => !alreadyStored.Contains(id)));

                // Transfer first, then run assignment availability checks as requested.
                var sourceTransferred = await _postPositionRepository
                    .TransferSourceAsync(req.SourcePostPositionId!.Value, actionEmployeeId, actionUsername)
                    .ConfigureAwait(false);

                if (!sourceTransferred)
                {
                    await vehicleService.ReleaseAllocationsAsync(newlyPersistedPositionIds).ConfigureAwait(false);
                    var failMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_UPDATE_FAILED);
                    AddBulkAssignErrorItem(result, req.PostID, req.DateOfWork, failMessage);
                    await SendPostTransferFailedNotificationToInitiatorAsync(
                        req.PostID,
                        req.DateOfWork,
                        source.EmployeeID,
                        targetEmployeeId,
                        failMessage).ConfigureAwait(false);
                    return result;
                }

                var assignResult = req.IsPlaceholder
                    ? await AssignPostToMe(req.PostID, req.DateOfWork, Guid.Empty, req.ConfirmConflict, isBulkAssign: true, postPositionId: req.SourcePostPositionId, isPlaceHolder: true, placeholder: _mapper.Map<PlaceholderDto>(placeholderItem!), enableInvoiceAndPayroll: placeholderItem!.EnableInvoiceAndPayroll).ConfigureAwait(false)
                    : await AssignPostToMe(req.PostID, req.DateOfWork, targetEmployeeId, req.ConfirmConflict, isBulkAssign: true, postPositionId: req.SourcePostPositionId).ConfigureAwait(false);

                if (assignResult.Status != 200 || assignResult.Data == null)
                {
                    await vehicleService.ReleaseAllocationsAsync(newlyPersistedPositionIds).ConfigureAwait(false);
                    var failMessage = string.IsNullOrWhiteSpace(assignResult.Message)
                        ? ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_ASSIGNMENT_UNAVAILABLE)
                        : assignResult.Message;
                    var rollbackStatus = await RollbackTransferredStatusAsync(
                        req.SourcePostPositionId!.Value,
                        source.PostPositionStatus,
                        actionEmployeeId,
                        actionUsername,
                        failMessage).ConfigureAwait(false);

                    AddBulkAssignErrorItem(
                        result,
                        req.PostID,
                        req.DateOfWork,
                        rollbackStatus,
                        hasTimeConflictRequiringConfirmation: assignResult.Status == (int)HttpStatusCode.Conflict);

                    await SendPostTransferFailedNotificationToInitiatorAsync(
                        req.PostID,
                        req.DateOfWork,
                        source.EmployeeID,
                        targetEmployeeId,
                        rollbackStatus).ConfigureAwait(false);
                    return result;
                }

                var created = assignResult.Data;

                if (created.PostPositionID is Guid newPositionId && newPositionId != Guid.Empty)
                {
                    await vehicleService
                        .CopyAllocationsAsync(req.SourcePostPositionId!.Value, newPositionId)
                        .ConfigureAwait(false);
                }

                await SendPostTransferNotificationsAsync(
                    req.PostID,
                    req.DateOfWork,
                    targetEmployeeId,
                    source.EmployeeID,
                    source.IsPlaceholder,
                    created.PostPositionID).ConfigureAwait(false);

                var postForMessage = await _postService.GetByIdAsync(req.PostID).ConfigureAwait(false);
                var successMessage = !string.IsNullOrEmpty(created.Message)
                    ? created.Message
                    : (postForMessage != null
                        ? ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_ASSIGN_SUCCESS, postForMessage.PostTitle)
                        : null);

                AddBulkAssignSuccessItem(result, created, successMessage);

                return result;
            }
            catch (WebApiException ex)
            {
                if (newlyPersistedPositionIds.Count > 0)
                {
                    using var rollbackScope = _scopeFactory.CreateScope();
                    await rollbackScope.ServiceProvider
                        .GetRequiredService<IPostPositionVehicleService>()
                        .ReleaseAllocationsAsync(newlyPersistedPositionIds)
                        .ConfigureAwait(false);
                }

                var errorMessage = string.IsNullOrWhiteSpace(ex.ErrorMessage)
                    ? ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_ASSIGNMENT_UNAVAILABLE)
                    : ex.ErrorMessage;
                AddBulkAssignErrorItem(result, req.PostID, req.DateOfWork, errorMessage);
                await SendPostTransferFailedNotificationToInitiatorAsync(
                    req.PostID,
                    req.DateOfWork,
                    source?.EmployeeID,
                    targetEmployeeId,
                    errorMessage).ConfigureAwait(false);
                return result;
            }
        }

        private async Task<string> RollbackTransferredStatusAsync(
            Guid sourcePostPositionId,
            PostPositionStatus originalStatus,
            Guid? actionEmployeeId,
            string? actionUsername,
            string message)
        {
            var rollbackCount = await _postPositionRepository.UpdateAsync(
                pp => pp.PostPositionID == sourcePostPositionId,
                pp =>
                {
                    pp.PostPositionStatus = originalStatus;
                    pp.EditedDate = DateTime.UtcNow;
                    pp.EditedUserID = actionEmployeeId;
                    pp.EditedBy = actionUsername;
                }).ConfigureAwait(false);

            if (rollbackCount > 0)
                return message;

            return $"{message} {ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_UPDATE_FAILED)}";
        }

        private static SuccessResult<List<BulkAssignPostPositionItemDto>> CreateBulkAssignResult()
        {
            return new SuccessResult<List<BulkAssignPostPositionItemDto>>
            {
                Data = new List<BulkAssignPostPositionItemDto>()
            };
        }

        private static void AddBulkAssignErrorItem(
            SuccessResult<List<BulkAssignPostPositionItemDto>> result,
            Guid postId,
            DateOnly dateOfWork,
            string? message,
            bool hasTimeConflictRequiringConfirmation = false,
            Guid? employeeId = null,
            string? employeeName = null)
        {
            result.Data.Add(new BulkAssignPostPositionItemDto
            {
                PostId = postId,
                DateOfWork = dateOfWork,
                PostPositionStatus = PostPositionStatus.Assigned,
                Message = message,
                HasTimeConflictRequiringConfirmation= hasTimeConflictRequiringConfirmation,
                EmployeeId= employeeId,
                EmployeeName= employeeName
            });
        }

        private static void AddBulkAssignSuccessItem(
            SuccessResult<List<BulkAssignPostPositionItemDto>> result,
            PostPositionDto dto,
            string? messageOverride = null)
        {
            result.Data.Add(new BulkAssignPostPositionItemDto(
                postId: dto.PostID,
                dateOfWork: dto.DateOfWork,
                postPositionStatus: dto.PostPositionStatus,
                message: messageOverride ?? dto.Message,
                postPositionId: dto.PostPositionID,
                postStartTime: dto.PostStartTime,
                lengthOfWork: dto.LengthOfWork));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestTitlesByDateRangeAsync(DateOnly startDate, int numberOfDays)
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return Array.Empty<RequestFilterOptionDto>();

            var (searchDate, _, searchEnd) = GetAvailablePostSearchWindow(startDate, numberOfDays);
            return await _repository.GetRequestTitlesByDateRange(searchDate, searchEnd, departmentId.Value).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestersByDateRangeAsync(DateOnly startDate, int numberOfDays)
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return Array.Empty<RequestFilterOptionDto>();

            var (searchDate, _, searchEnd) = GetAvailablePostSearchWindow(startDate, numberOfDays);
            return await _repository.GetRequestersByDateRange(searchDate, searchEnd, departmentId.Value).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetDashboardRequesterOptionsAsync(
            DateOnly startDate,
            int numberOfDays,
            string? searchTerm = null)
        {
            var items = await GetRequestersByDateRangeAsync(startDate, numberOfDays).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                items = items
                    .Where(x => x.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return items.Select(x => new SelectOptionDto
            {
                Id = x.Id,
                Label = x.Title
            });
        }

        public async Task<PagedList<AvailablePostDto>> GetAllAvailablePostRequest(
            SearchModel searchRequest,
            PageParam? pagination = null,
            bool isAvailablePost = false,
            Guid? employeeId = null)
        {
            var effectivePageParam = pagination ?? new PageParam();

            using var scope = _scopeFactory.CreateScope();
            var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();
            var requesterRestrictedListService = scope.ServiceProvider.GetRequiredService<IRequesterRestrictedListService>();
            var waitingPostPositionService = scope.ServiceProvider.GetRequiredService<IWaitingPostPositionService>();

            Guid? departmentId = _tokenParser.GetDepartmentID();
            var departmentpolicy = _tokenParser.GetDepartmentPolicy();
            var (searchDate, effectiveSearchDateTime, searchEnd) = GetAvailablePostSearchWindow(
                searchRequest.StartDate,
                searchRequest.NumberOfDays);
            if (departmentId == null)
            {
                return new PagedList<AvailablePostDto>(Enumerable.Empty<AvailablePostDto>(), effectivePageParam, 0);
            }

            var (subjectEmployeeId, viewingOtherEmployee) = await EmployeeJobsSubjectHelper.ResolveSubjectEmployeeAsync(
                employeeId,
                _tokenParser,
                _employeeService).ConfigureAwait(false);
            if (subjectEmployeeId == Guid.Empty)
            {
                return new PagedList<AvailablePostDto>(Enumerable.Empty<AvailablePostDto>(), effectivePageParam, 0);
            }

            // Admin JWT must not bypass release eligibility for another employee.
            var isDepartmentAdmin = !viewingOtherEmployee && _tokenParser.IsDepartmentAdmin();

            Expression<Func<Employee, object>>[] includes = { e => e.EmployeeType, e => e.EmployeeSkills };
            var employee = await _employeeService.GetByIdAsync(subjectEmployeeId, includes);
            if (employee == null || !employee.EmployeeID.HasValue || employee.EmployeeID.Value == Guid.Empty)
            {
                return new PagedList<AvailablePostDto>(Enumerable.Empty<AvailablePostDto>(), effectivePageParam, 0);
            }

            var restrictedEmployeeList = await requesterRestrictedListService.GetRequesterIdsByEmployeeId(subjectEmployeeId, searchDate, searchEnd);
            var employeeStatistics = await postPositionService.GetEmployeeStatistics(departmentpolicy, subjectEmployeeId, searchDate, effectiveSearchDateTime);
            var requests = await SearchRequests(searchDate, searchEnd, departmentId.Value, searchRequest.ReleaseAs,GetCurrentDepartmentDateTime());

            var extendedSearchStartDate = searchDate.AddDays(-1);
            var extendedSearchEndDate = searchEnd.AddDays(1);

            var employeeCurrentPostPositions = await postPositionService.GetPostPositionByEmployeeAndDateRangeAsync(subjectEmployeeId, extendedSearchStartDate, extendedSearchEndDate, departmentId);
            List<WaitingPostPositionDto> employeeWaitingPositions = new List<WaitingPostPositionDto>();
            employeeWaitingPositions = await waitingPostPositionService.GetWaitingPostPositionsByEmployeeAndDateRangeAsync(subjectEmployeeId,extendedSearchStartDate,extendedSearchEndDate,departmentId);
            // 3️⃣ Generate valid dates per request
            List<AvailablePostDto> availablePost = new();

            if (departmentpolicy != null && departmentpolicy.EnablePolicyManagement)
            {
                if (employeeStatistics.HasOutstandingReadyToClosePost(departmentpolicy))
                {
                    return new PagedList<AvailablePostDto>(Enumerable.Empty<AvailablePostDto>(), effectivePageParam, 0)
                    {
                        Error = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB),
                        HasOutstandingReadyToClosePost = true
                    };
                }
            }
            foreach (var request in requests)
            {
                var result = request.GetAvailablePosts(
                    employee, 
                    effectiveSearchDateTime.AddHours(-24),
                    effectiveSearchDateTime,
                    searchEnd, 
                    searchRequest.ReleaseAs, 
                    employeeCurrentPostPositions, 
                    employeeWaitingPositions ?? null, 
                    restrictedEmployeeList, 
                    departmentpolicy, 
                    employeeStatistics, 
                    searchRequest.DisplayWaitingList, 
                    isDepartmentAdmin,
                    validatePost:false);

                // Check if the result indicates outstanding ready-to-close post error
                if (result.HasOutstandingReadyToClosePost)
                {
                    return new PagedList<AvailablePostDto>(Enumerable.Empty<AvailablePostDto>(), effectivePageParam, 0)
                    {
                        Error = result.Message,
                        HasOutstandingReadyToClosePost = true
                    };
                }

                // Note: We don't collect warning messages here (e.g., max posts today)
                // Warnings are only shown when the user tries to assign a post, not when fetching available posts
                // This allows posts to be displayed even if the user has reached max posts today

                if (result.Posts != null && result.Posts.Any())
                {
                    availablePost.AddRange(result.Posts);
                }
            }

            if (searchRequest.Request.HasValue && searchRequest.Request.Value != Guid.Empty)
            {
                availablePost = availablePost.Where(p => p.RequestID == searchRequest.Request.Value).ToList();
            }

            if (searchRequest.Requester.HasValue && searchRequest.Requester.Value != Guid.Empty)
            {
                availablePost = availablePost.Where(p => p.RequesterID == searchRequest.Requester.Value).ToList();
            }

            if (searchRequest.PayCode.HasValue && searchRequest.PayCode.Value != Guid.Empty)
            {
                availablePost = availablePost.Where(p => p.PayCodeID == searchRequest.PayCode.Value).ToList();
            }

            availablePost = availablePost
                .OrderBy(r => r.DateOfWork)
                .ThenBy(r => r.PostStartTime)
                .ToList();

            var totalRecords = availablePost.Count;
            var pageSize = effectivePageParam.GetPageSize() ?? PageParam.MinPageSize;
            var page = effectivePageParam.Page ?? 1;
            var skip = (page - 1) * pageSize;
            var pagedItems = availablePost.Skip(skip).Take(pageSize).ToList();

            return new PagedList<AvailablePostDto>(pagedItems, effectivePageParam, totalRecords);
        }

        public async Task<PagedList<AvailablePostDto>> GetWeeklyAvailablePostRequestAsync(ReleaseAs release, PageParam? pagination = null)
        {
            var effectivePageParam = pagination ?? new PageParam();

            // Use department-local "today" to respect department week boundaries.
            var (departmentToday, _) = GetDateAsPerDepartmentTimeZone(DateOnly.FromDateTime(DateTime.UtcNow));

            var departmentPolicy = _tokenParser.GetDepartmentPolicy();
            var weekStartDay = DayOfWeek.Sunday;
            if (departmentPolicy != null)
            {
                try
                {
                    weekStartDay = departmentPolicy.OnDutyStartDayOfWeek.ToDayOfWeek();
                }
                catch
                {
                    weekStartDay = DayOfWeek.Sunday;
                }
            }

            var (weekStart, weekEnd) = departmentToday.GetDepartmentWeekRange(weekStartDay);

            var searchModel = new SearchModel
            {
                StartDate = weekStart,
                NumberOfDays = (weekEnd.DayNumber - weekStart.DayNumber) + 1,
                ReleaseAs = release
            };

            // Fetch a large set using existing available-post logic, then apply an explicit week filter + paging.
            // This mirrors the pattern used by GetWeeklyDashboardPostsAsync to guarantee the result is truly weekly.
            const int weeklyFetchMax = 10000;
            var fetchAll = new PageParam { Page = 1, PageSize = weeklyFetchMax };

            var raw = await GetAllAvailablePostRequest(searchModel, fetchAll, isAvailablePost: true);

            var weekly = raw.Data
                .Where(p => p.DateOfWork >= weekStart && p.DateOfWork <= weekEnd)
                .OrderBy(p => p.DateOfWork)
                .ThenBy(p => p.PostTitle)
                .ToList();

            var currentPage = effectivePageParam.Page.GetValueOrDefault(PageParam.MinPage);
            var pageSize = effectivePageParam.PageSize.GetValueOrDefault(PageParam.MinPageSize);
            var skip = (currentPage - 1) * pageSize;

            var paged = weekly
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return new PagedList<AvailablePostDto>(paged, effectivePageParam, weekly.Count);
        }
        public async Task<List<DateOnly>> GetPostPostitionsDatesByPostId(Guid postId)
        {
            Guid? departmentId = _tokenParser.GetDepartmentID();
            if (departmentId == null)
                return new List<DateOnly>();
            var request = await GetRequestByPostId(postId, departmentId.Value);
            var post = request.Posts.Where(r => r.PostID == postId).FirstOrDefault();
            var dates = request.CollectDates(request.StartDate, request.EndDate);
            var availableDates = dates.Where(date => post.HasAvailableSlotsForDate(postId, date)).ToList();
            return availableDates;
        }

        /// <inheritdoc />
        public async Task<List<DateOnly>> GetRosterDatesAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return new List<DateOnly>();

            var request = await GetByIdAsync(requestId).ConfigureAwait(false);
            if (request == null || request.RequestID == Guid.Empty)
                return new List<DateOnly>();

            // Include every schedule with a date window (not only ReleaseNow), so multi-schedule
            // requests return all calculated work dates. CollectDates(isBulkAssign: true) skips the
            // ReleaseNow filter but still requires StartDate/EndDate — pre-filter to avoid NREs.
            var schedulesWithDates = (request.RequestSchedules ?? Enumerable.Empty<RequestScheduleDto>())
                .Where(s => s.StartDate.HasValue && s.EndDate.HasValue)
                .ToList();
            request.RequestSchedules = schedulesWithDates;

            var dates = request.CollectDates(request.StartDate, request.EndDate, isBulkAssign: true);
            if (dates.Count > 0)
                return dates;

            // No schedule windows: fall back to the request start date so callers still get a default.
            if (request.StartDate != default)
                return new List<DateOnly> { request.StartDate };

            return new List<DateOnly>();
        }
        /// <summary>
        /// Clears <see cref="Post.IsAlcoholServedOrConsumed"/> on all posts under a request when alcohol permit is no longer required.
        /// Mirrors cascading cleanup used for vehicle/equipment flags.
        /// </summary>
        private async Task ClearPostAlcoholServedOrConsumedFlagsAsync(Guid requestId)
        {
            await _postService.UpdateAsync(
                p => p.RequestID == requestId && p.IsAlcoholServedOrConsumed,
                p => p.IsAlcoholServedOrConsumed = false).ConfigureAwait(false);
        }

        private static bool HasRequiresFlagChanged(bool? before, bool? after) =>
            after.HasValue && (before ?? false) != after.Value;

        /// <summary>
        /// Deletes RequestEquipment records based on RequiresVehicle and RequiresEquipment flags.
        /// Optimized: Single query with combined conditions, uses repository for direct database deletion.
        /// </summary>
        /// <param name="requestID">The RequestID to filter RequestEquipment records.</param>
        /// <param name="requiresVehicle">If false, deletes RequestEquipment where IsVehicle is true.</param>
        /// <param name="requiresEquipment">If false, deletes RequestEquipment where IsVehicle is false.</param>
        private async Task DeleteRequestEquipmentByFlagsAsync(Guid requestID, bool? requiresVehicle, bool? requiresEquipment)
        {
            await _requestEquipmentRepository.DeleteByFilterAsync(re => re.RequestID == requestID &&
                ((requiresVehicle == false && requiresEquipment == false) || // Delete all - no IsVehicle filter
                 (requiresVehicle == false && requiresEquipment != false && re.IsVehicle == true) || // Delete vehicles only
                 (requiresEquipment == false && requiresVehicle != false && re.IsVehicle == false))); // Delete non-vehicles only
        }

        private async Task SyncRequestDepartmentFeesAsync(
            Guid requestId,
            bool enableFees,
            IList<Guid>? selectedDepartmentFeeIds)
        {
            if (!enableFees)
            {
                await _requestDepartmentFeeRepository.DeleteByFilterAsync(r => r.RequestID == requestId)
                    .ConfigureAwait(false);
                return;
            }

            if (selectedDepartmentFeeIds == null)
                return;

            var desiredIds = selectedDepartmentFeeIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToHashSet();

            var existing = (await _requestDepartmentFeeRepository.GetAllAsync(
                r => r.RequestID == requestId,
                null,
                null).ConfigureAwait(false)).ToList();

            var toRemove = existing.Where(r => !desiredIds.Contains(r.DepartmentFeeID)).ToList();
            if (toRemove.Count > 0)
            {
                await _requestDepartmentFeeRepository.DeleteByFilterAsync(
                    r => r.RequestID == requestId && !desiredIds.Contains(r.DepartmentFeeID))
                    .ConfigureAwait(false);
            }

            var existingIds = existing.Select(r => r.DepartmentFeeID).ToHashSet();
            var toAdd = desiredIds.Where(id => !existingIds.Contains(id)).ToList();
            if (toAdd.Count == 0)
                return;

            var newRows = toAdd.Select(feeId => new RequestDepartmentFee
            {
                RequestDepartmentFeeID = Guid.Empty,
                RequestID = requestId,
                DepartmentFeeID = feeId
            }).ToList();

            await _requestDepartmentFeeRepository.BulkUpsertAsync(newRows, "RequestDepartmentFeeID")
                .ConfigureAwait(false);
        }

        private async Task RemoveApprovalDataByTypeAsync(Guid requestID, ApprovalType approvalType)
        {
            var existingRequestApprovalRoutings = (await _requestApprovalRoutingService.GetAllAsync(
                x => x.RequestID == requestID,
                null,
                null)).ToList();

            if (!existingRequestApprovalRoutings.Any())
                return;

            var approverIds = existingRequestApprovalRoutings
                .Select(x => x.ApproverID)
                .Distinct()
                .ToList();

            if (!approverIds.Any())
                return;

            var routingGroupIdsToDelete = (await _routingGroupService.GetAllAsync(
                x => approverIds.Contains(x.RoutingGroupId) && x.ApprovalType == approvalType,
                null,
                null))
                .Where(x => x.RoutingGroupId.HasValue)
                .Select(x => x.RoutingGroupId!.Value)
                .ToHashSet();

            if (!routingGroupIdsToDelete.Any())
                return;

            var existingApprovalDetails = (await _approvalDetailRepository.GetAllAsync(
                x => x.RequestID == requestID &&
                    (x.Type == approvalType || routingGroupIdsToDelete.Contains(x.RouteToID)),
                null,
                null)).ToList();

            foreach (var detail in existingApprovalDetails)
            {
                if (detail.ApprovalDetailID != Guid.Empty)
                {
                    await _approvalDetailRepository.DeleteAsync<Guid>(detail.ApprovalDetailID);
                }
            }

            var requestApprovalRoutingsToDelete = existingRequestApprovalRoutings
                .Where(x => routingGroupIdsToDelete.Contains(x.ApproverID))
                .ToList();

            foreach (var routing in requestApprovalRoutingsToDelete)
            {
                if (routing.RequestApprovalRouteID.HasValue)
                {
                    await _requestApprovalRoutingService.DeleteAsync<Guid>(routing.RequestApprovalRouteID.Value);
                }
            }

            // Re-rank the remaining approval routes after deletion so the request keeps
            // a contiguous approval sequence (1, 2, 3, ...) for UI ordering and future inserts.
            var remainingRequestApprovalRoutings = existingRequestApprovalRoutings
                .Where(x => !routingGroupIdsToDelete.Contains(x.ApproverID))
                .OrderBy(x => x.Rank)
                .Select(x => new RequestApprovalRoutingDto
                {
                    RequestApprovalRouteID = x.RequestApprovalRouteID,
                    RequestID = x.RequestID,
                    ApproverID = x.ApproverID,
                    Rank = x.Rank,
                    RequestApproverHasFinalAuthority = x.RequestApproverHasFinalAuthority
                })
                .ToList();

            if (remainingRequestApprovalRoutings.Any())
            {
                await _requestApprovalRoutingService.BulkUpsertAsync(remainingRequestApprovalRoutings);
            }
        }
        public async Task<RosterResponseDto> GetRosterInformationAsync(Guid postId, DateOnly dateOfWork)
        {
            // Normalize date according to department timezone
            var normalizedDate = NormalizeDateToDepartmentToday(dateOfWork);

            // Get request ID from post ID
            var post = await _postService.GetByIdAsync(postId);
            if (post?.RequestID == null)
                return new RosterResponseDto();

            return await GetRosterInformationByRequestAsync(post.RequestID.Value, normalizedDate);
        }

        public async Task<RosterResponseDto> GetRosterInformationByRequestAsync(Guid requestID, DateOnly dateOfWork)
        {
            // Normalize date according to department timezone
            var normalizedDate = NormalizeDateToDepartmentToday(dateOfWork);

            // Get request with requester, all posts, and postpositions filtered by date
            var requestEntity = await _repository.GetRequestWithPostsAndPositionsForRosterAsync(requestID, normalizedDate);
            if (requestEntity == null)
                return new RosterResponseDto();

            // Map entity to DTOs
            var requestDto = _mapper.Map<RequestDto>(requestEntity);
            var allPosts = requestDto.Posts ?? new List<PostDto>();

            // Build recursive hierarchy to set Level property
            var orderedPosts = _postService.BuildPostHierarchy(allPosts);

            // Ensure Request and Requester are populated in all posts
            foreach (var postDto in orderedPosts)
            {
                if (postDto.Request == null)
                {
                    postDto.Request = requestDto;
                }
                else if (postDto.Request.Requester == null && requestDto.Requester != null)
                {
                    postDto.Request.Requester = requestDto.Requester;
                }
            }

            // Use orderedPosts instead of allPosts for the rest of the method
            allPosts = orderedPosts;

            var loaded = await _viewDetailsEquipmentRepository
                .LoadAsync(requestID, postId: null, normalizedDate, loadRankContext: true)
                .ConfigureAwait(false);

            var requestEquipmentDtos = _mapper.Map<List<RequestEquipmentDto>>(loaded.RequestEquipment);
            var postEquipmentDtos = _mapper.Map<List<PostEquipmentDto>>(loaded.PostEquipment);
            var lookup = ViewDetailsDepartmentEquipmentLookupBuilder.FromLoadedEquipment(
                loaded.RequestEquipment,
                loaded.PostEquipment);
            var requestPositions = _mapper.Map<List<PostPositionDto>>(loaded.RequestPositions);
            var hireDateByEmployeeId = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(loaded.EmployeeTenures);
            var postEquipmentByPostId = postEquipmentDtos
                .Where(pe => pe.PostID.HasValue && pe.PostID.Value != Guid.Empty)
                .GroupBy(pe => pe.PostID!.Value)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<PostEquipmentDto>)g.ToList());

            var departmentNow = GetCurrentDepartmentDateTime();
            var rosterPositionIds = requestPositions
                .Where(p => p.PostPositionID.HasValue && p.PostPositionID.Value != Guid.Empty)
                .Select(p => p.PostPositionID!.Value)
                .Distinct()
                .ToList();
            HashSet<Guid> frozenPositionIds;
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int> allocationCounts;
            using (var rosterVehicleScope = _scopeFactory.CreateScope())
            {
                frozenPositionIds = await rosterVehicleScope.ServiceProvider
                    .GetRequiredService<IPostPositionVehicleService>()
                    .GetAllocatedPositionIdsAsync(rosterPositionIds)
                    .ConfigureAwait(false);

                allocationCounts = await rosterVehicleScope.ServiceProvider
                    .GetRequiredService<IPostPositionVehicleRepository>()
                    .GetAllocationCountsByPostAndDateAsync(
                        requestPositions.Select(p => p.PostID).Where(id => id != Guid.Empty).Distinct().ToList(),
                        new[] { normalizedDate })
                    .ConfigureAwait(false);
            }

            var vehicleNameByPostAndEmployee = new Dictionary<Guid, IReadOnlyDictionary<Guid, string?>>();
            foreach (var post in orderedPosts)
            {
                if (post.PostID is not Guid postId || postId == Guid.Empty)
                    continue;

                var postPositions = requestPositions.Where(p => p.PostID == postId).ToList();
                var postEquipment = postEquipmentByPostId.TryGetValue(postId, out var postList)
                    ? postList
                    : Array.Empty<PostEquipmentDto>();
                loaded.PostDepartmentEquipmentIdsByPostId.TryGetValue(postId, out var postOverrides);

                var namesByPositionId = new Dictionary<Guid, string?>();
                foreach (var position in post.PostPositions ?? Enumerable.Empty<PostPositionDto>())
                {
                    if (position.DateOfWork != normalizedDate
                        || position.PostPositionID is not Guid positionId
                        || positionId == Guid.Empty)
                    {
                        continue;
                    }

                    IReadOnlyList<ViewDetailsEquipmentLine> lines;
                    var isPassed = position.IsPostPositionDateTimePassed(departmentNow);
                    var inFreeze = position.IsPostPositionInFreezeWindow(departmentNow);
                    if (isPassed || inFreeze || frozenPositionIds.Contains(positionId) || position.IsAllocationFinalized)
                    {
                        var pastDetails = new ViewDetailsDto();
                        await PopulatePastPostPositionViewDetailsEquipmentAsync(
                                pastDetails,
                                positionId,
                                requestID,
                                postId,
                                normalizedDate,
                                position.EmployeeID == Guid.Empty ? null : position.EmployeeID,
                                position.PostPositionStatus,
                                CancellationToken.None,
                                isAllocationFinalized: position.IsAllocationFinalized)
                            .ConfigureAwait(false);
                        lines = pastDetails.Equipments;
                    }
                    else
                    {
                        if (position.EmployeeID == Guid.Empty)
                            continue;

                        lines = ViewDetailsEquipmentLines.BuildLines(
                            requestEquipmentDtos,
                            postEquipment,
                            lookup,
                            showVehicle: true,
                            showEquipment: false,
                            position.EmployeeID,
                            requestPositions,
                            postPositions,
                            filterByViewerRank: true,
                            hireDateByEmployeeId,
                            normalizedDate,
                            postOverrides,
                            loaded.EmployeeTenures,
                            allocationCounts: allocationCounts);
                    }

                    var names = lines
                        .Where(l => l.IsVehicle && !string.IsNullOrWhiteSpace(l.EquipmentTypeName))
                        .Select(l => l.EquipmentTypeName!)
                        .Distinct()
                        .ToList();
                    namesByPositionId[positionId] = names.Count == 0 ? null : string.Join(", ", names);
                }

                vehicleNameByPostAndEmployee[postId] = namesByPositionId;
            }

            // Get all employee IDs from postpositions
            var employeeIds = allPosts
                .SelectMany(p => p.PostPositions ?? Enumerable.Empty<PostPositionDto>())
                .Where(pp => pp.EmployeeID != Guid.Empty)
                .Select(pp => pp.EmployeeID)
                .Distinct()
                .ToList();

            // Get employees
            var employees = employeeIds.Any()
                ? (await _employeeService.GetAllAsync(e => employeeIds.Contains(e.EmployeeID))).ToList()
                : new List<EmployeeDto>();

            var employeeDict = employees
                .Where(e => e.EmployeeID.HasValue)
                .ToDictionary(e => e.EmployeeID!.Value);
            return requestDto.BuildRosterResponse(normalizedDate, employeeDict, vehicleNameByPostAndEmployee);

        }
        private static ExportFileDto BuildRosterExcelExport(RosterResponseDto response)
        {
            List<BaseExcelGenerator> excelElements = new();

            excelElements.Add(new RosterHeader(response.RequesterName ?? "",response.RosterDate));
            foreach (var table in response.Tables)
            {
                excelElements.Add(new RosterSection(table.PostTitle,table.PostDescription,table.PayCodeValue));
                excelElements.Add(new RosterTable(table));
            }
            var export =new ExcelExport(ExcelConstants.DefaultSheetName,excelElements);
            return new ExportFileDto
            {
                FileName =$"{response.RequesterName}.xlsx",
                FileBytes =export.Export()
            };
        }
        public async Task<ExportFileDto> ExportRosterExcelAsync(Guid requestID,DateOnly dateOfWork)
        {
            var response = await GetRosterInformationByRequestAsync(requestID,dateOfWork);
            return BuildRosterExcelExport(response);
        }
        public async Task<ExportFileDto> ExportRosterExcelByPostAsync(Guid postId, DateOnly dateOfWork)
        {
            var response = await GetRosterInformationAsync(postId, dateOfWork);
            return BuildRosterExcelExport(response);
        }

        /// <inheritdoc cref="IRequestService.GetViewDetailsAsync"/>
        public async Task<ViewDetailsDto> GetViewDetailsAsync(
            Guid? postPositionId,
            Guid? postId,
            Guid? requestId,
            bool? isSpecialEvent,
            DateOnly? dateOfWork = null,
            CancellationToken cancellationToken = default)
        {
            var response = new ViewDetailsDto();
            if (postPositionId.HasValue && postPositionId.Value != Guid.Empty)
                await PopulatePostPositionViewDetailsAsync(response, postPositionId.Value, dateOfWork, cancellationToken).ConfigureAwait(false);
            else
                await PopulateAvailableJobViewDetailsAsync(response, postId, requestId, isSpecialEvent, dateOfWork, cancellationToken).ConfigureAwait(false);

            return response;
        }

        private async Task PopulateAvailableJobViewDetailsAsync(
            ViewDetailsDto response,
            Guid? postId,
            Guid? requestId,
            bool? isSpecialEvent,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            response.IsSpecialEvent = isSpecialEvent;
            var loadPost = postId.HasValue
                && postId.Value != Guid.Empty
                && isSpecialEvent != true;

            var initialRequestId = requestId.HasValue && requestId.Value != Guid.Empty
                ? requestId.Value
                : (Guid?)null;

            // Sequential loads: scoped DbContext is not safe for concurrent operations (see EF Core threading guidance).
            PostDto? fullPost = null;
            if (loadPost)
                fullPost = await LoadPostForAvailablePostDetailsAsync(postId!.Value, cancellationToken).ConfigureAwait(false);

            RequestDto? fullRequest = null;
            if (initialRequestId.HasValue)
                fullRequest = await LoadRequestForAvailablePostDetailsAsync(initialRequestId.Value, cancellationToken).ConfigureAwait(false);

            if (fullRequest == null
                && fullPost?.RequestID is Guid fromPostRequestId
                && fromPostRequestId != Guid.Empty)
            {
                fullRequest = await LoadRequestForAvailablePostDetailsAsync(fromPostRequestId, cancellationToken).ConfigureAwait(false);
            }

            if (fullPost != null
                && fullPost.RequestID.HasValue
                && fullPost.RequestID.Value != Guid.Empty
                && fullRequest != null
                && fullRequest.RequesterID.HasValue
                && fullRequest.RequesterID.Value != Guid.Empty)
                await TryPopulateViewDetailsFilesAsync(response, fullRequest, cancellationToken).ConfigureAwait(false);

            response.Post = MapViewDetailsPostFromPost(fullPost);
            await PopulateViewDetailsPayCodeAsync(response.Post, cancellationToken).ConfigureAwait(false);
            response.Request = MapViewDetailsRequest(fullRequest);
            await PopulateViewDetailsRequesterAsync(response, fullRequest).ConfigureAwait(false);
            response.ApplyAvailablePositionsForAvailablePost(fullPost, dateOfWork);

            await PopulateAvailableJobVehicleEquipmentFromPostAndRequestAsync(
                    response,
                    fullPost,
                    fullRequest,
                    dateOfWork,
                    cancellationToken)
                .ConfigureAwait(false);

            if (isSpecialEvent == true
                && fullRequest != null
                && fullRequest.RequestID != Guid.Empty)
            {
                var (specialAvailable, specialTotal) = await ComputeSpecialEventPositionCountsAsync(
                        fullRequest.RequestID,
                        dateOfWork,
                        cancellationToken)
                    .ConfigureAwait(false);
                response.AvailablePositions = specialAvailable;
                response.TotalPostPositions = specialTotal;
                response.SpecialEventJobDetailsMessage = ModelValidationErrorCodes.GetErrorMessage(
                    ModelErrorCodes.VIEW_DETAILS_SPECIAL_EVENT_JOB_DETAILS,
                    specialAvailable);
            }

            if (fullPost?.PostID is { } latestNotePostId && latestNotePostId != Guid.Empty)
                await TryPopulateLatestPostNoteAsync(response, latestNotePostId, cancellationToken).ConfigureAwait(false);
        }

        private async Task PopulateAvailableJobVehicleEquipmentFromPostAndRequestAsync(
            ViewDetailsDto response,
            PostDto? fullPost,
            RequestDto? fullRequest,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            response.ApplyVehicleEquipmentFlagsForAvailableJob(fullPost, fullRequest);

            var effRequestId = fullPost?.RequestID ?? fullRequest?.RequestID;
            var hasRequestId = effRequestId.HasValue && effRequestId.Value != Guid.Empty;
            var postEntityId = fullPost?.PostID;

            if (hasRequestId)
            {
                await PopulateViewDetailsEquipmentListsAsync(
                        response,
                        response.ShowVehicle,
                        response.ShowEquipment,
                        effRequestId!.Value,
                        postEntityId,
                        dateOfWork,
                        viewerEmployeeId: null,
                        filterByViewerRank: false,
                        cancellationToken)
                    .ConfigureAwait(false);

                if (dateOfWork.HasValue)
                    ViewDetailsEquipmentLines.ApplyShowFlagsFromEquipmentLines(response);
            }
            else
                response.ClearEquipmentLists();
        }

        private async Task TryPopulateViewDetailsFilesAsync(
            ViewDetailsDto response,
            RequestDto fullRequest,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fileResult = await _uploadFileService
                .GetFilesOrFileAsync(
                    fullRequest.RequestID,
                    fullRequest.RequesterID!.Value,
                    fileId: null,
                    cancellationToken)
                .ConfigureAwait(false);

            if (fileResult?.List is { Count: > 0 } list)
                response.Files = _mapper.Map<List<ViewDetailsAttachment>>(list.ToList());
        }

        private async Task PopulateViewDetailsRequesterAsync(ViewDetailsDto response, RequestDto? fullRequest)
        {
            RequesterDto? fullRequester = null;
            if (fullRequest?.RequesterID is Guid requesterId && requesterId != Guid.Empty)
                fullRequester = await _requesterService.GetByIdAsync(requesterId).ConfigureAwait(false);
            response.Requester = MapViewDetailsRequester(fullRequester);
        }

        private async Task PopulatePostPositionViewDetailsAsync(
            ViewDetailsDto response,
            Guid postPositionId,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();

            var withPost = await postPositionService.GetPostPositionWithPostDetailsByIdAsync(postPositionId).ConfigureAwait(false);
            if (withPost == null)
                return;

            var postPosition = await postPositionService
                .GetByIdAsync(postPositionId, x => x.Post!, x => x.Employee!, x => x.PostCallSign!)
                .ConfigureAwait(false);
            if (postPosition == null)
                return;

            var postPositionVehicleService = scope.ServiceProvider.GetRequiredService<IPostPositionVehicleService>();
            var departmentNow = GetCurrentDepartmentDateTime();
            var isPassed = postPosition.IsPostPositionDateTimePassed(departmentNow);
            var inFreezeWindow = postPosition.IsPostPositionInFreezeWindow(departmentNow, hoursBeforeStart: 2);
            var isAllocationFinalized = postPosition.IsAllocationFinalized;
            var hasFrozenAllocations = (await postPositionVehicleService.GetFilteredCountAsync(
                x => x.PostPositionID == postPositionId && !x.IsDeleted).ConfigureAwait(false)) > 0;

            if (isPassed || hasFrozenAllocations || inFreezeWindow || isAllocationFinalized)
            {
                // Already-finalized positions are locked to PostPositionVehicle (even if empty).
                // Only attempt a freeze write when the flag is still open and no rows exist yet.
                if (!isAllocationFinalized
                    && !hasFrozenAllocations
                    && postPosition.Post != null
                    && postPosition.Post.Request != null)
                {
                    var departmentId = postPosition.Post.Request.DepartmentID;
                    var master = await scope.ServiceProvider.GetRequiredService<IEmployeeService>()
                        .GetMasterEmployeeByDepartmentIdAsync(departmentId).ConfigureAwait(false);
                    if (master?.EmployeeID != null && master.EmployeeID != Guid.Empty)
                    {
                        var masterDisplayName = PersonNameFormatHelper.FormatLastNameCommaFirstName(master.LastName, master.FirstName) ?? "Master System";
                        var startTime = postPosition.PostStartTime ?? postPosition.Post?.PostStartTime ?? TimeOnly.MinValue;
                        var startDateTime = postPosition.DateOfWork!.Value.ToDateTime(startTime);
                        var freezeCutoffTime = startDateTime > departmentNow ? startDateTime.AddHours(2) : departmentNow.AddHours(2);

                        await postPositionVehicleService.FinalizeStartedAllocationsForDepartmentAsync(
                            departmentId,
                            freezeCutoffTime,
                            master.EmployeeID.Value,
                            masterDisplayName,
                            cancellationToken).ConfigureAwait(false);

                        // Finalize marks eligible positions in DB; refresh so empty freeze rows stay locked.
                        var refreshed = await postPositionService.GetByIdAsync(postPositionId).ConfigureAwait(false);
                        if (refreshed != null)
                            postPosition.IsAllocationFinalized = refreshed.IsAllocationFinalized;
                    }
                }

                await PopulatePastPostPositionViewDetailsAsync(
                        response,
                        postPosition,
                        withPost,
                        dateOfWork,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await PopulateCurrentPostPositionViewDetailsAsync(
                        response,
                        postPosition,
                        withPost,
                        dateOfWork,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// View details for an assigned post position whose scheduled start has not yet passed (department time).
        /// Vehicle and equipment lines use live rank/configuration rules.
        /// </summary>
        private async Task PopulateCurrentPostPositionViewDetailsAsync(
            ViewDetailsDto response,
            PostPositionDto postPosition,
            PostTypeResponseDto withPost,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            await ApplyPostPositionViewDetailsShellAsync(
                    response,
                    postPosition,
                    withPost,
                    dateOfWork,
                    populateEquipmentAsync: async (shellResponse, requestId, postId, workDate, viewerEmployeeId) =>
                    {
                        await PopulateViewDetailsEquipmentListsAsync(
                                shellResponse,
                                showVehicle: true,
                                showEquipment: true,
                                requestId,
                                postId,
                                workDate,
                                viewerEmployeeId,
                                filterByViewerRank: true,
                                cancellationToken,
                                historicalViewerStatus: postPosition.PostPositionStatus)
                            .ConfigureAwait(false);
                        ViewDetailsEquipmentLines.ApplyShowFlagsFromEquipmentLines(shellResponse);
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// View details for an assigned post position whose work date/start time has passed (department time),
        /// is in the freeze window, or has <see cref="PostPositionDto.IsAllocationFinalized"/> set.
        /// Vehicle/equipment lines come only from finalized <see cref="PostPositionVehicle"/> rows when finalized;
        /// otherwise empty freeze rows may still fall back to live rank rules until freeze has run.
        /// </summary>
        private async Task PopulatePastPostPositionViewDetailsAsync(
            ViewDetailsDto response,
            PostPositionDto postPosition,
            PostTypeResponseDto withPost,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            await ApplyPostPositionViewDetailsShellAsync(
                    response,
                    postPosition,
                    withPost,
                    dateOfWork,
                    populateEquipmentAsync: async (shellResponse, requestId, postId, workDate, viewerEmployeeId) =>
                    {
                        await PopulatePastPostPositionViewDetailsEquipmentAsync(
                                shellResponse,
                                postPosition.PostPositionID!.Value,
                                requestId,
                                postId,
                                workDate,
                                viewerEmployeeId,
                                postPosition.PostPositionStatus,
                                cancellationToken,
                                isAllocationFinalized: postPosition.IsAllocationFinalized)
                            .ConfigureAwait(false);
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ApplyPostPositionViewDetailsShellAsync(
            ViewDetailsDto response,
            PostPositionDto postPosition,
            PostTypeResponseDto withPost,
            DateOnly? dateOfWork,
            Func<ViewDetailsDto, Guid, Guid, DateOnly, Guid?, Task> populateEquipmentAsync,
            CancellationToken cancellationToken)
        {
            if (withPost.PostID != Guid.Empty)
            {
                var postWithPositions = await LoadPostForAvailablePostDetailsAsync(withPost.PostID, cancellationToken).ConfigureAwait(false);
                response.ApplyAvailablePositionsForAvailablePost(postWithPositions, dateOfWork ?? postPosition.DateOfWork);
            }

            var effRequestId = withPost.RequestID;
            var hasRequestId = effRequestId.HasValue && effRequestId.Value != Guid.Empty;
            var postEntityId = withPost.PostID;

            var fullRequest = hasRequestId
                ? await LoadRequestForAvailablePostDetailsAsync(effRequestId!.Value, cancellationToken).ConfigureAwait(false)
                : null;

            if (hasRequestId && postEntityId != Guid.Empty)
            {
                var viewerEmployeeId = postPosition.EmployeeID != Guid.Empty
                    ? postPosition.EmployeeID
                    : _tokenParser.GetEmployeeID();
                var workDate = dateOfWork ?? postPosition.DateOfWork ?? withPost.DateOfWork;
                if (!workDate.HasValue)
                {
                    response.ClearEquipmentLists();
                    response.ShowVehicle = false;
                    response.ShowEquipment = false;
                }
                else
                {
                    await populateEquipmentAsync(
                            response,
                            effRequestId!.Value,
                            postEntityId,
                            workDate.Value,
                            viewerEmployeeId)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                response.ClearEquipmentLists();
                response.ShowVehicle = false;
                response.ShowEquipment = false;
            }

            response.Post = MapViewDetailsPostFromPost(postPosition.Post)
                ?? MapViewDetailsPostFromWithPost(withPost);
            await PopulateViewDetailsPayCodeAsync(response.Post, cancellationToken).ConfigureAwait(false);
            response.PostPosition = MapViewDetailsPostPosition(postPosition);
            response.Request = MapViewDetailsRequest(fullRequest);
            await PopulateViewDetailsRequesterAsync(response, fullRequest).ConfigureAwait(false);

            if (hasRequestId
                && fullRequest != null
                && fullRequest.RequesterID.HasValue
                && fullRequest.RequesterID.Value != Guid.Empty)
            {
                await TryPopulateViewDetailsFilesAsync(response, fullRequest, cancellationToken).ConfigureAwait(false);
            }

            if (postEntityId != Guid.Empty)
                await TryPopulateLatestPostNoteAsync(response, postEntityId, cancellationToken).ConfigureAwait(false);
        }

        private async Task PopulatePastPostPositionViewDetailsEquipmentAsync(
            ViewDetailsDto response,
            Guid postPositionId,
            Guid requestId,
            Guid postId,
            DateOnly workDate,
            Guid? viewerEmployeeId,
            PostPositionStatus historicalViewerStatus,
            CancellationToken cancellationToken,
            bool isAllocationFinalized = false)
        {
            using var scope = _scopeFactory.CreateScope();
            var postPositionVehicleService = scope.ServiceProvider.GetRequiredService<IPostPositionVehicleService>();

            // Matched on the post position alone. The frozen rows record what was actually issued, so post
            // equipment edits made after the job cannot drop these lines or send the job back to rank rules.
            var frozenAllocations = (await postPositionVehicleService.GetAllAsync(
                allocation => allocation.PostPositionID == postPositionId
                    && !allocation.IsDeleted,
                null,
                null).ConfigureAwait(false)).ToList();

            if (frozenAllocations.Count > 0)
            {
                var loaded = await _viewDetailsEquipmentRepository
                    .LoadAsync(requestId, postId, workDate, false, cancellationToken)
                    .ConfigureAwait(false);

                var departmentEquipmentIds = frozenAllocations
                    .Select(allocation => allocation.DepartmentEquipmentID.GetValueOrDefault())
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                var departmentEquipmentById = departmentEquipmentIds.Count > 0
                    ? (await _departmentEquipmentService.GetAllAsync(
                        equipment => departmentEquipmentIds.Contains(equipment.DepartmentEquipmentID),
                        new Expression<Func<DepartmentEquipment, object>>[] { equipment => equipment.EquipmentType! },
                        null).ConfigureAwait(false))
                    .Where(equipment => equipment.DepartmentEquipmentID.HasValue)
                    .ToDictionary(equipment => equipment.DepartmentEquipmentID!.Value)
                    : new Dictionary<Guid, DepartmentEquipmentDto>();

                var configuredQuantityMap = new Dictionary<Guid, int>();
                foreach (var pe in loaded.PostEquipment)
                {
                    if (pe.DepartmentEquipmentID != Guid.Empty && pe.EquipmentQuantity > 0)
                        configuredQuantityMap[pe.DepartmentEquipmentID] = pe.EquipmentQuantity;
                }

                response.Equipments = ViewDetailsEquipmentLines.BuildLinesFromFinalizedAllocations(
                    frozenAllocations,
                    departmentEquipmentById,
                    configuredQuantityMap);
                ViewDetailsEquipmentLines.ApplyShowFlagsFromEquipmentLines(response);
                return;
            }

            // Finalized positions are locked to PostPositionVehicle only: empty freeze table means hide lines.
            if (isAllocationFinalized)
            {
                response.Equipments = Array.Empty<ViewDetailsEquipmentLine>();
                response.ShowVehicle = false;
                response.ShowEquipment = false;
                return;
            }

            // Only reached when freeze has not finalized this position yet (flag still open, no rows).
            var fallbackLoaded = await _viewDetailsEquipmentRepository
                .LoadAsync(requestId, postId, workDate, false, cancellationToken)
                .ConfigureAwait(false);

            if (fallbackLoaded.PostEquipment.Count > 0)
            {
                var fallbackResponse = new ViewDetailsDto();
                await PopulateViewDetailsEquipmentListsAsync(
                        fallbackResponse,
                        showVehicle: true,
                        showEquipment: true,
                        requestId,
                        postId,
                        workDate,
                        viewerEmployeeId,
                        filterByViewerRank: true,
                        cancellationToken,
                        historicalViewerStatus)
                    .ConfigureAwait(false);

                response.Equipments = fallbackResponse.Equipments;
                ViewDetailsEquipmentLines.ApplyShowFlagsFromEquipmentLines(response);
                return;
            }

            response.Equipments = Array.Empty<ViewDetailsEquipmentLine>();
            response.ShowVehicle = false;
            response.ShowEquipment = false;
        }

        private async Task TryPopulateLatestPostNoteAsync(
            ViewDetailsDto response,
            Guid postId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var note = await _postNoteService.GetLatestNoteByPostIdAsync(postId).ConfigureAwait(false);
            if (note != null)
                response.LatestPostNote = _mapper.Map<ViewDetailsLatestPostNote>(note);
        }

        private async Task PopulateViewDetailsPayCodeAsync(
            ViewDetailsPostInfo? post,
            CancellationToken cancellationToken)
        {
            if (post?.PayCodeId is not { } payCodeId || payCodeId == Guid.Empty)
                return;

            cancellationToken.ThrowIfCancellationRequested();
            using var scope = _scopeFactory.CreateScope();
            var payCodeService = scope.ServiceProvider.GetRequiredService<IPayCodeService>();
            var payCode = await payCodeService.GetByIdAsync(payCodeId).ConfigureAwait(false);
            if (payCode == null)
                return;

            post.PayCodeValue = payCode.PayCodeValue;
            post.PayCodeDescription = payCode.PayCodeDescription;
        }

        private ViewDetailsPostInfo? MapViewDetailsPostFromPost(PostDto? post)
        {
            if (post?.PostID is not { } id || id == Guid.Empty)
                return null;
            return _mapper.Map<ViewDetailsPostInfo>(post);
        }

        private ViewDetailsPostInfo? MapViewDetailsPostFromWithPost(PostTypeResponseDto? withPost)
        {
            if (withPost == null || withPost.PostID == Guid.Empty)
                return null;
            return _mapper.Map<ViewDetailsPostInfo>(withPost);
        }

        private ViewDetailsRequestInfo? MapViewDetailsRequest(RequestDto? request)
        {
            if (request == null || request.RequestID == Guid.Empty)
                return null;
            return _mapper.Map<ViewDetailsRequestInfo>(request);
        }

        private ViewDetailsRequesterInfo? MapViewDetailsRequester(RequesterDto? requester)
        {
            if (requester?.RequesterID is not { } id || id == Guid.Empty)
                return null;
            return _mapper.Map<ViewDetailsRequesterInfo>(requester);
        }

        private ViewDetailsPostPositionInfo? MapViewDetailsPostPosition(PostPositionDto? position)
        {
            if (position?.PostPositionID is not { } id || id == Guid.Empty)
                return null;
            return _mapper.Map<ViewDetailsPostPositionInfo>(position);
        }

        /// <summary>
        /// Special-event pool: sums across all posts on the request. <paramref name="dateOfWork"/> set → available = sum of <see cref="PostDto.GetAvailableSlotsForDate"/>, total = sum of <see cref="PostDto.TotalPostPositions"/>; omitted → both equal sum of <see cref="PostDto.TotalPostPositions"/> (legacy special-event display).
        /// </summary>
        private async Task<(int Available, int Total)> ComputeSpecialEventPositionCountsAsync(
            Guid requestId,
            DateOnly? dateOfWork,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Expression<Func<Post, object>>[] includes = { p => p.PostPositions };
            var posts = (await _postService.GetAllAsync(x => x.RequestID == requestId, includes, null).ConfigureAwait(false))
                .ToList();
            if (posts.Count == 0)
                return (0, 0);

            var ordered = _postService.BuildPostHierarchy(posts);
            var total = ordered.Sum(p => p.TotalPostPositions);
            if (!dateOfWork.HasValue)
                return (total, total);

            var available = ordered.Sum(p => p.GetAvailableSlotsForDate(dateOfWork.Value));
            return (available, total);
        }

        private async Task PopulateViewDetailsEquipmentListsAsync(
            ViewDetailsDto response,
            bool showVehicle,
            bool showEquipment,
            Guid requestId,
            Guid? postId,
            DateOnly? dateOfWork,
            Guid? viewerEmployeeId,
            bool filterByViewerRank,
            CancellationToken cancellationToken,
            PostPositionStatus? historicalViewerStatus = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!dateOfWork.HasValue || requestId == Guid.Empty)
            {
                response.ClearEquipmentLists();
                return;
            }

            if (!showVehicle && !showEquipment)
            {
                response.ClearEquipmentLists();
                return;
            }

            var loaded = await _viewDetailsEquipmentRepository
                .LoadAsync(requestId, postId, dateOfWork, filterByViewerRank, cancellationToken)
                .ConfigureAwait(false);

            var requestDtos = _mapper.Map<List<RequestEquipmentDto>>(loaded.RequestEquipment);
            var postDtos = _mapper.Map<List<PostEquipmentDto>>(loaded.PostEquipment);
            var lookup = ViewDetailsDepartmentEquipmentLookupBuilder.FromLoadedEquipment(
                loaded.RequestEquipment,
                loaded.PostEquipment);

            IReadOnlyList<PostPositionDto> requestPositions = Array.Empty<PostPositionDto>();
            IReadOnlyList<PostPositionDto> postPositions = Array.Empty<PostPositionDto>();
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null;
            IEnumerable<EmployeeTenure>? tenures = null;
            if (filterByViewerRank)
            {
                requestPositions = _mapper.Map<List<PostPositionDto>>(loaded.RequestPositions);
                postPositions = postId is Guid scopedPostId && scopedPostId != Guid.Empty
                    ? requestPositions.Where(p => p.PostID == scopedPostId).ToList()
                    : requestPositions;
                hireDateByEmployeeId = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(loaded.EmployeeTenures);
                tenures = loaded.EmployeeTenures;
            }

            IReadOnlySet<Guid>? postOverrideDepartmentEquipmentIds = null;
            if (postId is Guid overridePostId && overridePostId != Guid.Empty
                && loaded.PostDepartmentEquipmentIdsByPostId.TryGetValue(overridePostId, out var overrideIds))
            {
                postOverrideDepartmentEquipmentIds = overrideIds;
            }

            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts = null;
            if (filterByViewerRank)
            {
                var postIds = requestPositions
                    .Select(p => p.PostID)
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();
                using var vehicleScope = _scopeFactory.CreateScope();
                allocationCounts = await vehicleScope.ServiceProvider
                    .GetRequiredService<IPostPositionVehicleRepository>()
                    .GetAllocationCountsByPostAndDateAsync(postIds, new[] { dateOfWork.Value }, cancellationToken)
                    .ConfigureAwait(false);
            }

            response.Equipments = ViewDetailsEquipmentLines.BuildLines(
                requestDtos,
                postDtos,
                lookup,
                showVehicle,
                showEquipment,
                viewerEmployeeId,
                requestPositions,
                postPositions,
                filterByViewerRank,
                hireDateByEmployeeId,
                dateOfWork,
                postOverrideDepartmentEquipmentIds,
                tenures,
                historicalViewerStatus,
                allocationCounts);
        }

        private async Task<PostDto?> LoadPostForAvailablePostDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Match SearchRequests → Posts.PostPositions so GetAvailableSlotsForDate matches the available-posts grid.
            return await _postService.GetByIdAsync(id, p => p.PostPositions).ConfigureAwait(false);
        }

        private async Task<RequestDto?> LoadRequestForAvailablePostDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await GetByIdAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Same window as <see cref="GetAllAvailablePostRequest"/>: department-local normalization of
        /// <paramref name="startDate"/> plus end = <see cref="DateHelper.GetEndDate"/>(normalized start, <paramref name="numberOfDays"/>).
        /// </summary>
        private (DateOnly SearchDate, DateTime EffectiveSearchDateTime, DateOnly SearchEndDate) GetAvailablePostSearchWindow(
            DateOnly startDate,
            int numberOfDays)
        {
            var (searchDate, effectiveSearchDateTime) = GetDateAsPerDepartmentTimeZone(startDate);
            var searchEnd = DateHelper.GetEndDate(searchDate, numberOfDays);
            return (searchDate, effectiveSearchDateTime, searchEnd);
        }

        /// <inheritdoc />
        public async Task<PagedList<OpenJobsDashboardDto>> GetOpenJobsDashboardAsync(DateOnly startDate, DateOnly endDate, PageParam? pagination = null, string? searchTerm = null, string? statusFilter = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var departmentId = _tokenParser.GetDepartmentID();
            var pageParam = pagination ?? new PageParam();
            if (departmentId == null)
                return new PagedList<OpenJobsDashboardDto>(Enumerable.Empty<OpenJobsDashboardDto>(), pageParam, 0);

            if (endDate < startDate)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE));
            }

            var (rangeStart, _) = GetDateAsPerDepartmentTimeZone(startDate);
            var rangeEnd = endDate;

            var requests = await SearchRequests(rangeStart, rangeEnd, departmentId.Value, ReleaseAs.Open,
                    GetCurrentDepartmentDateTime())
                .ConfigureAwait(false);
            var rows = new List<(Guid RequestID, string RequestTitle, string ProjectNumber, int? FiscalYear, string RequesterName, DateOnly DateOfWork, OpenJobDashboardRowDto Job)>();

            foreach (var request in requests)
            {
                if (request.Posts == null || !request.Posts.Any())
                    continue;

                foreach (var post in request.Posts)
                {
                    if (!post.PostID.HasValue || post.TotalPostPositions <= 0)
                        continue;

                    var dates = request.CollectDates(rangeStart, rangeEnd);
                    var workingDates = post.GetUnblockedDates(dates);

                    foreach (var date in workingDates)
                    {
                        if (!request.IsReleaseAsForDate(date, ReleaseAs.Open))
                            continue;

                        var openSlots = post.GetAvailableSlotsForDate(date);
                        var cancellationPending = CountParentCancellationPendingForDate(post, date);
                        var booked = post.TotalPostPositions - openSlots;
                        var filled = booked;
                        if (filled < 0)
                            filled = 0;

                        rows.Add((request.RequestID, (request.Title ?? string.Empty).Trim(), (request.ProjectNumber ?? string.Empty).Trim(), request.FiscalYear, (request.Requester?.Name ?? string.Empty).Trim(), date, new OpenJobDashboardRowDto
                        {
                            PostID = post.PostID!.Value,
                            PostTitle = post.PostTitle ?? string.Empty,
                            TotalPostPositions = post.TotalPostPositions,
                            DayOfWeek = date.DayOfWeek.ToString(),
                            StartDate = date,
                            StartTime = post.PostStartTime,
                            EndTime = post.PostEndTime,
                            Open = openSlots,
                            Filled = filled,
                            PendingCancellation = cancellationPending,
                        }));
                    }
                }
            }

            var grouped = rows
                .GroupBy(x => new { x.RequestID, x.RequestTitle, x.ProjectNumber, x.FiscalYear, x.RequesterName, x.DateOfWork })
                .Select(g =>
                {
                    var jobs = g
                        .Select(x => x.Job)
                        .OrderBy(r => r.StartTime)
                        .ThenBy(r => r.PostTitle, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    return new OpenJobsDashboardDto(
                        requestId: g.Key.RequestID,
                        requestTitle: g.Key.RequestTitle,
                        projectNumber: g.Key.ProjectNumber,
                        fiscalYear: g.Key.FiscalYear,
                        requesterName: g.Key.RequesterName,
                        dateOfWork: g.Key.DateOfWork,
                        openJobs: jobs,
                        totalPostPositions: jobs.Sum(j => j.TotalPostPositions),
                        totalOpen: jobs.Sum(j => j.Open),
                        totalFilled: jobs.Sum(j => j.Filled),
                        totalCancel: jobs.Sum(j => j.PendingCancellation));
                })
                .OrderBy(g => g.RequesterName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => g.RequestTitle, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => g.DateOfWork)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                grouped.RemoveAll(g =>
                {
                    if (g.RequestTitle != null && g.RequestTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                        return false;

                    g.OpenJobs.RemoveAll(j => !j.PostTitle.Contains(term, StringComparison.OrdinalIgnoreCase));
                    return g.OpenJobs.Count == 0;
                });
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                var filters = statusFilter
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(f => f.ToUpperInvariant())
                    .ToHashSet();

                var filterOpen = filters.Contains("OPEN");
                var filterFilled = filters.Contains("FILLED");
                var filterCancels = filters.Contains("CANCELS");

                if (filterOpen || filterFilled || filterCancels)
                {
                    grouped.RemoveAll(g =>
                        !(filterOpen && g.TotalOpen > 0) &&
                        !(filterFilled && g.TotalFilled > 0) &&
                        !(filterCancels && g.TotalCancel > 0));
                }
            }

            var totalRecords = grouped.Count;
            var skip = pageParam.Offset ?? 0;
            var take = Math.Max(1, pageParam.Limit ?? PageParam.MinLimit);
            var pageRows = grouped.Skip(skip).Take(take).ToList();

            return new PagedList<OpenJobsDashboardDto>(pageRows, pageParam, totalRecords)
            {
                Error = totalRecords == 0
                    ? ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.OPEN_JOBS_DASHBOARD_NO_RECORDS)
                    : null,
            };
        }

        /// <inheritdoc />
        public async Task<PagedList<TenureJobsDashboardDto>> GetTenureJobsDashboardAsync(
            DateOnly startDate,
            DateOnly endDate,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pageParam = pagination ?? new PageParam();

            if (endDate < startDate)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE));
            }

            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return new PagedList<TenureJobsDashboardDto>(Enumerable.Empty<TenureJobsDashboardDto>(), pageParam, 0);

            var requests = (await SearchRequests(
                    startDate,
                    endDate,
                    departmentId.Value,
                    ReleaseAs.TenureBased,
                    GetCurrentDepartmentDateTime())
                .ConfigureAwait(false))
                .ToList();

            var tenureSchedulesByRequestId = requests.ToDictionary(
                r => r.RequestID,
                r => (r.RequestSchedules ?? new List<RequestScheduleDto>())
                    .Where(s => s.RequestStatus == RequestStatus.ReleaseNow && s.ReleaseAs == ReleaseAs.TenureBased)
                    .ToList());

            var rows = new List<(Guid RequestID, string RequestTitle, string ProjectNumber, int? FiscalYear, string RequesterName, DateOnly DateOfWork, DateOnly? ProcessDate, TimeOnly? ProcessTime, TenureJobsDashboardRowDto Job)>();
            foreach (var (request, post, date) in EnumerateRequestPostWorkingDates(
                         requests,
                         startDate,
                         endDate,
                         ReleaseAs.TenureBased,
                         skipSpecialEventRequests: false))
            {
                var tenureSchedules = tenureSchedulesByRequestId.GetValueOrDefault(request.RequestID)
                    ?? new List<RequestScheduleDto>();

                var matchedSchedule = tenureSchedules
                    .Where(s =>
                        s.StartDate.HasValue &&
                        s.EndDate.HasValue &&
                        s.StartDate.Value <= date &&
                        s.EndDate.Value >= date)
                    .OrderByDescending(s => s.StartDate)
                    .ThenByDescending(s => s.ProcessDate)
                    .ThenByDescending(s => s.ProcessTime)
                    .FirstOrDefault();

                rows.Add((request.RequestID, (request.Title ?? string.Empty).Trim(), (request.ProjectNumber ?? string.Empty).Trim(), request.FiscalYear, (request.Requester?.Name ?? string.Empty).Trim(), date, matchedSchedule?.ProcessDate, matchedSchedule?.ProcessTime, new TenureJobsDashboardRowDto(
                    postId: post.PostID!.Value,
                    postTitle: post.PostTitle,
                    dateOfWork: date,
                    startTime: post.PostStartTime,
                    endTime: post.PostEndTime,
                    minimumHoursRequired: post.PostMinimumHoursRequired,
                    location: post.PostLocation,
                    totalPostPositions: post.TotalPostPositions)));
            }

            var grouped = rows
                .GroupBy(x => new { x.RequestID, x.RequestTitle, x.ProjectNumber, x.FiscalYear, x.RequesterName, x.DateOfWork, x.ProcessDate, x.ProcessTime })
                .Select(g => new TenureJobsDashboardDto(
                    requestId: g.Key.RequestID,
                    requestTitle: g.Key.RequestTitle,
                    projectNumber: g.Key.ProjectNumber,
                    fiscalYear: g.Key.FiscalYear,
                    requesterName: g.Key.RequesterName,
                    dateOfWork: g.Key.DateOfWork,
                    processDate: g.Key.ProcessDate,
                    processTime: g.Key.ProcessTime,
                    tenureJobs: g
                        .Select(x => x.Job)
                        .OrderBy(r => r.StartTime)
                        .ThenBy(r => r.PostTitle, StringComparer.OrdinalIgnoreCase)
                        .ToList()))
                .OrderBy(g => g.RequesterName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => g.RequestTitle, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => g.DateOfWork)
                .ToList();

            var totalRecords = grouped.Count;
            var skip = pageParam.Offset ?? 0;
            var take = Math.Max(1, pageParam.Limit ?? PageParam.MinLimit);
            var pageRows = grouped.Skip(skip).Take(take).ToList();

            return new PagedList<TenureJobsDashboardDto>(pageRows, pageParam, totalRecords);
        }

        private static IEnumerable<(RequestDto Request, PostDto Post, DateOnly DateOfWork)> EnumerateRequestPostWorkingDates(
            IEnumerable<RequestDto> requests,
            DateOnly rangeStart,
            DateOnly rangeEnd,
            ReleaseAs releaseAs,
            bool skipSpecialEventRequests)
        {
            foreach (var request in requests)
            {
                if (skipSpecialEventRequests && request.IsSpecialEvent == true)
                    continue;

                if (request.Posts == null || !request.Posts.Any())
                    continue;

                var dates = request.CollectDates(rangeStart, rangeEnd);
                foreach (var post in request.Posts)
                {
                    if (!post.PostID.HasValue || post.PostID.Value == Guid.Empty || post.TotalPostPositions <= 0)
                        continue;

                    var workingDates = post.GetUnblockedDates(dates);
                    foreach (var date in workingDates)
                    {
                        if (!request.IsReleaseAsForDate(date, releaseAs))
                            continue;

                        yield return (request, post, date);
                    }
                }
            }
        }

        /// <summary>
        /// Tenure lookback ends on the tenure schedule process date that covers <paramref name="dateOfWork"/>.
        /// When that process date is missing, the work date is the window end.
        /// </summary>
        private async Task<DateOnly> ResolveTenureStatisticsWindowEndAsync(Guid requestId, DateOnly dateOfWork)
        {
            if (requestId == Guid.Empty)
                return dateOfWork;

            var schedules = (await _requestScheduleService.GetAllAsync(
                    s => s.RequestId == requestId &&
                         s.ReleaseAs == ReleaseAs.TenureBased &&
                         s.ProcessDate.HasValue)
                .ConfigureAwait(false)).ToList();

            var matchedSchedule = schedules
                .Where(s =>
                    (s.StartDate == null || dateOfWork >= s.StartDate) &&
                    (s.EndDate == null || dateOfWork <= s.EndDate))
                .OrderByDescending(s => s.StartDate)
                .ThenByDescending(s => s.ProcessDate)
                .ThenByDescending(s => s.ProcessTime)
                .FirstOrDefault();

            return matchedSchedule?.ProcessDate ?? dateOfWork;
        }

        /// <inheritdoc />
        public async Task<List<TenureInterestedEmployeeRowDto>> GetTenureExpressInterestDashboardAsync(
            Guid postId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "Department not found in token.");
            }

            var post = await _postService.GetByIdAsync(postId).ConfigureAwait(false);
            if (post?.RequestID == null || post.RequestID == Guid.Empty)
            {
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    $"Post with id {postId} not found.");
            }

            var releaseAs = await _repository.GetLatestReleaseAsForRequestAsync(post.RequestID.Value).ConfigureAwait(false);
            //if (releaseAs != ReleaseAs.TenureBased)
            //{
            //    throw new WebApiException(
            //        HttpStatusCode.BadRequest,
            //        ((int)HttpStatusCode.BadRequest).ToString(),
            //        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TENURE_EXPRESS_INTEREST_NOT_TENURE_RELEASE));
            //}

            using var scope = _scopeFactory.CreateScope();
            var waitingPostPositionService = scope.ServiceProvider.GetRequiredService<IWaitingPostPositionService>();
            var tenureEmployeeStatisticsService = scope.ServiceProvider.GetRequiredService<ITenureEmployeeStatisticsService>();
            var departmentPolicy = _tokenParser.GetDepartmentPolicy();
            if (departmentPolicy == null)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "Department policy not found in token.");
            }

            var waitingRows = await waitingPostPositionService
                .GetForPostsForJobAsync(new List<Guid> { postId }, cancellationToken)
                .ConfigureAwait(false);

            waitingRows = waitingRows
                .Where(r => r.PostID == postId && r.DateOfWork == dateOfWork && (r.ProcessingStatus == ProcessingStatus.Pending || r.ProcessingStatus == ProcessingStatus.Failed))
                .ToList();

            if (waitingRows.Count == 0)
                return new List<TenureInterestedEmployeeRowDto>();

            var lookbackColumns = tenureEmployeeStatisticsService.BuildLookbackColumns(departmentPolicy.TenureRulesJson);

            var allEmployeeIds = waitingRows.Select(r => r.EmployeeID).Distinct().ToList();
            Dictionary<Guid, EmployeeDto> employeeById = new();
            if (allEmployeeIds.Count > 0)
            {
                var employees = (await _employeeService.GetAllAsync(
                        e => allEmployeeIds.Contains(e.EmployeeID),
                        new Expression<Func<Employee, object>>[] { e => e.EmployeeType },
                        null)
                    .ConfigureAwait(false)).ToList();
                employeeById = employees
                    .Where(e => e.EmployeeID.HasValue)
                    .ToDictionary(e => e.EmployeeID!.Value);
            }

            var groups = waitingRows
                .Where(r => r.PostID.HasValue)
                .GroupBy(r => new { PostId = r.PostID!.Value, r.DateOfWork })
                .OrderBy(g => g.Key.DateOfWork)
                .ThenBy(g => g.Key.PostId);

            var interestedEmployees = new List<TenureInterestedEmployeeRowDto>();
            foreach (var group in groups)
            {
                var rowsInGroup = group.ToList();
                var empIdsForStats = rowsInGroup.Select(r => r.EmployeeID).Distinct().ToList();
                var windowEnd = await ResolveTenureStatisticsWindowEndAsync(post.RequestID.Value, group.Key.DateOfWork).ConfigureAwait(false);
                var statisticsRows = await tenureEmployeeStatisticsService.GetTenureEmployeeStatisticsAsync(
                        departmentPolicy,
                        empIdsForStats,
                        windowEnd,
                        departmentId,
                        cancellationToken)
                    .ConfigureAwait(false);

                var orderedWaitingRows = TenureEmployeeStatisticsHelper.OrderWaitingRowsByTenureStatistics(
                    rowsInGroup,
                    statisticsRows);

                var interested = new List<TenureInterestedEmployeeRowDto>();
                foreach (var (w, statRow) in orderedWaitingRows)
                {
                    employeeById.TryGetValue(w.EmployeeID, out var emp);

                    interested.Add(new TenureInterestedEmployeeRowDto
                    {
                        WaitingPostPositionId = w.WaitingPostPositionID,
                        EmployeeID = w.EmployeeID,
                        FirstName = emp?.FirstName,
                        LastName = emp?.LastName,
                        EmployeeNumber = emp?.EmployeeNumber,
                        EmployeeTypeName = emp?.EmployeeType?.EmployeeTypeShort ?? emp?.EmployeeType?.EmployeeTypeLong,
                        OrderNumber = w.OrderNumber,
                        HasAppliedInTenure = w.HasAppliedInTenure,
                        StatisticsByLabel = TenureEmployeeStatisticsHelper.MapLevelValuesToLabels(statRow, lookbackColumns),
                        tenureType = statRow.TenureType
                    });
                }
                interestedEmployees.AddRange(interested);
            }

            return interestedEmployees;
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessingResultDto> ProcessTenureWaitingListForWaitingPostManualAsync(
            Guid waitingPostPositionId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (waitingPostPositionId == Guid.Empty)
                throw new WebApiException(HttpStatusCode.BadRequest,((int)HttpStatusCode.BadRequest).ToString(),ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY));

            var departmentId = _tokenParser.GetDepartmentID();

            using var scope = _scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var waitingPostPositionService = sp.GetRequiredService<IWaitingPostPositionService>();
            var postPositionService = sp.GetRequiredService<IPostPositionService>();
            var departmentPolicyService = sp.GetRequiredService<IDepartmentPolicyService>();
            var tenureEmployeeStatisticsService = sp.GetRequiredService<ITenureEmployeeStatisticsService>();
            var jobAuditService = sp.GetRequiredService<IJobAuditService>();
            var logger = sp.GetRequiredService<ILogger<RequestService>>();

            WaitingPostPositionDto? tenureRow = await waitingPostPositionService.GetByIdAsync(waitingPostPositionId).ConfigureAwait(false);
            if (tenureRow == null)
                throw new WebApiException( HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), 
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_NOT_FOUND_BY_ID, waitingPostPositionId));

            if (!tenureRow.PostID.HasValue || tenureRow.PostID.Value == Guid.Empty)
                throw new WebApiException( HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_POST_POSITION_POST_ID_REQUIRED));

            var post = await _postService.GetPostWithRequestAsync(tenureRow.PostID.Value).ConfigureAwait(false);
            if (post?.RequestID == null || post.RequestID == Guid.Empty || post.Request == null)
                throw new WebApiException( HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_ASSOCIATED_REQUEST_NOT_FOUND));

            if (post.Request.DepartmentID != departmentId.Value)
                throw new WebApiException( HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TENURE_WAITING_SINGLE_EMPLOYEE_REQUEST_CONTEXT_MISSING));

            Guid requestId = post.RequestID.Value;
            Guid employeeId = tenureRow.EmployeeID;

            var releaseAs = await _repository.GetLatestReleaseAsForRequestAsync(requestId).ConfigureAwait(false);
            if (releaseAs != ReleaseAs.TenureBased)
                throw new WebApiException( HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TENURE_EXPRESS_INTEREST_NOT_TENURE_RELEASE));

            var employee = await _employeeService.GetByIdAsync(employeeId).ConfigureAwait(false);
            if (employee == null)
                throw new WebApiException( HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(),
                    $"Employee with ID {employeeId} was not found.");


            if (tenureRow.ProcessingStatus != ProcessingStatus.Pending)
                throw new WebApiException( HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    "This waiting post position is not pending processing.");

            RequestScheduleProcessDueRequestDto? context = await _repository
                .GetTenureWaitingListProcessingContextReadOnlyAsync(requestId, departmentId.Value, cancellationToken).ConfigureAwait(false);

            if (context == null)
                throw new WebApiException( HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TENURE_WAITING_SINGLE_EMPLOYEE_REQUEST_CONTEXT_MISSING));

            var result = new RequestScheduleProcessingResultDto(requestId);
            var correlationId = Activity.Current?.Id;

            try
            {
                var rowsAll = await waitingPostPositionService.GetForPostsForJobAsync(context.PostIDs, cancellationToken).ConfigureAwait(false);

                tenureRow = rowsAll.FirstOrDefault(r => r.WaitingPostPositionID == waitingPostPositionId) ?? tenureRow;

                var departmentPolicy = await departmentPolicyService
                    .GetDepartmentPolicyConfigurationByDepartmentId(context.DepartmentID).ConfigureAwait(false);

                if (departmentPolicy == null)
                {
                    var skipReason = $"Department policy is not configured for DepartmentID {context.DepartmentID}. Processing skipped.";
                    result.MarkFailed(skipReason);
                    result.FinalizeResult();
                    var policySkipChangedOn = DateTime.UtcNow;
                    var policyPlaceholder = BuildTenureManualPlaceholderRow(context);
                    var policyAudit = jobAuditService.CreateWaitingPostPositionJobAudit(
                        policyPlaceholder,
                        context.DepartmentID,
                        employeeId,
                        policyPlaceholder.DateOfWork,
                        AuditAction.UPDATE,
                        policySkipChangedOn,
                        correlationId,
                        ProcessingStatus.Failed,
                        skipReason,
                        entityIdOverride: requestId.ToString("D"));
                    await jobAuditService.EnqueueAuditAsync(policyAudit, cancellationToken).ConfigureAwait(false);
                    return result;
                }

                var postId = tenureRow.PostID!.Value;
                var dateOfWork = tenureRow.DateOfWork;

                var employeeIdsForDate = rowsAll
                    .Where(r =>
                        r.DateOfWork == dateOfWork &&
                        r.PostID == postId &&
                        r.EmployeeID != Guid.Empty)
                    .Select(r => r.EmployeeID)
                    .Distinct()
                    .ToList();

                var windowEnd = await ResolveTenureStatisticsWindowEndAsync(requestId, dateOfWork).ConfigureAwait(false);
                var orderedEmployeesForDate = await tenureEmployeeStatisticsService
                    .GetTenureEmployeeStatisticsAsync(departmentPolicy, employeeIdsForDate, windowEnd, context.DepartmentID, cancellationToken)
                    .ConfigureAwait(false);

                await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.InProgress, "Processing started", markProcessed: false);
                var available = await CheckAvailablePostForJobAsync(postId, dateOfWork, employeeId, context.DepartmentID, context.TimeZoneId, departmentPolicy, false).ConfigureAwait(false);

                if (available.HasError)
                {
                    var failureMessage = available.Message ?? string.Empty;
                    result.AddFailure(employeeId, failureMessage);
                    await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.Failed, failureMessage, markProcessed: true);
                    var failChangedOn = DateTime.UtcNow;
                    var failAudit = jobAuditService.CreateWaitingPostPositionJobAudit(
                        tenureRow,
                        context.DepartmentID,
                        employeeId,
                        dateOfWork,
                        AuditAction.UPDATE,
                        failChangedOn,
                        correlationId,
                        ProcessingStatus.Failed,
                        failureMessage);
                    await jobAuditService.EnqueueAuditAsync(failAudit, cancellationToken).ConfigureAwait(false);
                    result.FinalizeResult();
                    result.MarkFailed(failureMessage);
                    await ReorderTenureManualWaitingPositionsAsync(waitingPostPositionService, postId, dateOfWork, orderedEmployeesForDate, cancellationToken).ConfigureAwait(false);
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), failureMessage);
                }

                var copied = await postPositionService.CopyTenureWaitingToPostPositionsAsync(new List<WaitingPostPositionDto> { tenureRow }, cancellationToken).ConfigureAwait(false);

                if (copied > 0)
                {
                    result.AddSuccess(employeeId);
                    await waitingPostPositionService.UpdateProcessingStatusAsync(
                        waitingPostPositionId,
                        ProcessingStatus.Complete,
                        "job assigned successfully",
                        markProcessed: true);
                    await waitingPostPositionService.MarkWaitingRowAssignedAsync(waitingPostPositionId, cancellationToken: cancellationToken).ConfigureAwait(false);
                    var assignOkChangedOn = DateTime.UtcNow;
                    var assignOkAudit = jobAuditService.CreateWaitingPostPositionJobAudit(
                        tenureRow,
                        context.DepartmentID,
                        employeeId,
                        dateOfWork,
                        AuditAction.UPDATE,
                        assignOkChangedOn,
                        correlationId,
                        ProcessingStatus.Complete,
                        string.Empty);
                    await jobAuditService.EnqueueAuditAsync(assignOkAudit, cancellationToken).ConfigureAwait(false);
                    result.FinalizeResult();
                    result.MarkSuccessed("Tenure waiting row assigned for employee.");
                }
                else
                {
                    var copyFailDetail = string.IsNullOrWhiteSpace(available.Message)
                        ? "CopyTenureWaitingToPostPositionsAsync returned 0 rows."
                        : available.Message;
                    result.AddFailure(employeeId, copyFailDetail);
                    await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.Failed, copyFailDetail, markProcessed: true);
                    var copyFailChangedOn = DateTime.UtcNow;
                    var copyAudit = jobAuditService.CreateWaitingPostPositionJobAudit(
                        tenureRow,
                        context.DepartmentID,
                        employeeId,
                        dateOfWork,
                        AuditAction.UPDATE,
                        copyFailChangedOn,
                        correlationId,
                        ProcessingStatus.Failed,
                        copyFailDetail);
                    await jobAuditService.EnqueueAuditAsync(copyAudit, cancellationToken).ConfigureAwait(false);
                    result.FinalizeResult();
                    result.MarkFailed(copyFailDetail);
                    await ReorderTenureManualWaitingPositionsAsync(waitingPostPositionService, postId, dateOfWork, orderedEmployeesForDate, cancellationToken).ConfigureAwait(false);
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), copyFailDetail);
                }

                await ReorderTenureManualWaitingPositionsAsync(waitingPostPositionService, postId, dateOfWork, orderedEmployeesForDate, cancellationToken).ConfigureAwait(false);
            }
            catch (WebApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Tenure waiting-list processing failed for Request {RequestID} WaitingPostPosition {WaitingPostPositionId}", requestId, waitingPostPositionId);
                result.MarkFailed(ex.Message);
                result.FinalizeResult();
                if (context != null && requestId != Guid.Empty)
                {
                    var exceptionChangedOn = DateTime.UtcNow;
                    var exceptionPlaceholder = BuildTenureManualPlaceholderRow(context);
                    var exceptionAudit = jobAuditService.CreateWaitingPostPositionJobAudit(
                        exceptionPlaceholder,
                        context.DepartmentID,
                        employeeId != Guid.Empty ? employeeId : null,
                        exceptionPlaceholder.DateOfWork,
                        AuditAction.UPDATE,
                        exceptionChangedOn,
                        correlationId,
                        ProcessingStatus.Failed,
                        ex.Message,
                        entityIdOverride: requestId.ToString("D"));
                    await jobAuditService.EnqueueAuditAsync(exceptionAudit, cancellationToken).ConfigureAwait(false);
                }
            }

            return result;
        }

        private static Task ReorderTenureManualWaitingPositionsAsync(
            IWaitingPostPositionService waitingPostPositionService,
            Guid postId,
            DateOnly dateOfWork,
            IReadOnlyList<TenureEmployeeStatisticsDto>? orderedEmployees,
            CancellationToken cancellationToken)
        {
            if (orderedEmployees == null || orderedEmployees.Count == 0)
                return Task.CompletedTask;

            var finalOrderedIds = orderedEmployees.Select(e => e.EmployeeID).ToList();
            return waitingPostPositionService.ReorderWaitingPositionsByGivenOrderNumbersAsync(postId, dateOfWork, finalOrderedIds, cancellationToken);
        }

        private static WaitingPostPositionDto BuildTenureManualPlaceholderRow(RequestScheduleProcessDueRequestDto request)
        {
            var title = string.IsNullOrWhiteSpace(request.RequestTitle) ? "(no title)" : request.RequestTitle.Trim();
            return new WaitingPostPositionDto
            {
                Post = new PostDto { PostTitle = title },
                DateOfWork = DateOnly.FromDateTime(request.ProcessLocalDateTime),
                RequestId = request.RequestID,
            };
        }

        private static int CountParentCancellationPendingForDate(PostDto post, DateOnly date)
        {
            if (post.PostPositions == null)
                return 0;

            return post.PostPositions.Count(p =>
                p.ParentPostPositionId == null &&
                p.IsSameWorkDate(date) &&
                p.PostPositionStatus == PostPositionStatus.CancellationPending);
        }

        private async Task TransferPostPositionsForOneTimeDateChangeAsync(
            IPostPositionService postPositionService,
            Guid requestId,
            DateOnly oldDate,
            DateOnly newDate,
            IReadOnlyList<Guid> transferPostPositionIds)
        {
            var transferIdSet = transferPostPositionIds.ToHashSet();
            var allAssignedPositions = await postPositionService
                .GetAssignedRealEmployeesByRequestAndDateAsync(requestId, oldDate)
                .ConfigureAwait(false);
            if (allAssignedPositions.Count == 0)
                return;

            var positionsToTransfer = allAssignedPositions
                .Where(p => transferIdSet.Contains(p.PostPositionID))
                .ToList();
            if (positionsToTransfer.Count != transferPostPositionIds.Count)
                ThrowInvalidTransferPostPositionIds();

            if (await _postPositionRepository.CancelPostPositionsByIdsAsync(
                    allAssignedPositions.Select(p => p.PostPositionID).ToList()).ConfigureAwait(false)
                != allAssignedPositions.Count)
                ThrowInvalidTransferPostPositionIds();

            var transferDtos = positionsToTransfer
                .Select(p => BuildPostPositionForDateTransfer(p, newDate))
                .ToList();
            if (transferDtos.Count > 0)
                await postPositionService.BulkUpsertAsync(transferDtos).ConfigureAwait(false);

            foreach (var position in positionsToTransfer.GroupBy(p => p.EmployeeID!.Value).Select(g => g.First()))
            {
                await SendPostTransferNotificationsAsync(
                    position.PostID,
                    newDate,
                    position.EmployeeID!.Value,
                    position.EmployeeID!.Value,
                    position.IsPlaceholder,
                    position.PostPositionID,
                    CancellationToken.None).ConfigureAwait(false);
            }

            void ThrowInvalidTransferPostPositionIds() =>
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.REQUEST_ONE_TIME_DATE_CHANGE_INVALID_TRANSFER_POST_POSITION_IDS));
        }

        private PostPositionDto BuildPostPositionForDateTransfer(PostPosition source, DateOnly newDate)
        {
            var dto = _mapper.Map<PostPositionDto>(source);
            dto.PostPositionID = Guid.Empty;
            dto.PostPositionCode = null;
            dto.DateOfWork = newDate;
            dto.PostPositionStatus = PostPositionStatus.Assigned;
            dto.ReasonForCancel = null;
            dto.ParentPostPositionId = null;
            dto.SplitPostPositionID = null;
            dto.IsSplit = false;
            dto.WasUnclosed = false;
            dto.Employee = null!;
            dto.Post = null!;
            return dto;
        }

        private static void ValidateOneTimeFutureDateChangeAsync(
            OneTimeDateChangeReviewDataDto reviewData,
            DateOnly newDate)
        {
            var requestPostIds = reviewData.AssignedOnOldDate.Select(row => row.PostID).ToHashSet();
            var slotsByEmployee = reviewData.EmployeeSlotsOnNewDate
                .Where(slot => !requestPostIds.Contains(slot.PostID))
                .GroupBy(slot => slot.EmployeeID)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<OneTimeDateChangeConflictSlotRowDto>)g.ToList());

            var assignments = reviewData.AssignedOnOldDate
                .Select(row =>
                {
                    slotsByEmployee.TryGetValue(row.EmployeeID, out var otherOnNewDate);
                    return new RequestOneTimeDateChangeAssignmentDto(
                        row,
                        HasOneTimeDateChangeTimeConflict(row, newDate, otherOnNewDate));
                })
                .ToList();

            var message = ModelValidationErrorCodes.GetErrorMessage(
                ModelValidationErrorCodes.REQUEST_ONE_TIME_DATE_CHANGE_ASSIGNMENTS_WARNING);
            throw new WebApiException(
                HttpStatusCode.Conflict,
                ((int)HttpStatusCode.Conflict).ToString(),
                message,
                new RequestOneTimeDateChangeConflictDto
                {
                    Message = message,
                    AssignedPostPositions = assignments
                });
        }

        private static bool HasOneTimeDateChangeTimeConflict(
            OneTimeDateChangeAssignedRowDto assignment,
            DateOnly newDate,
            IReadOnlyList<OneTimeDateChangeConflictSlotRowDto>? otherSlotsOnNewDate)
        {
            if (otherSlotsOnNewDate == null || otherSlotsOnNewDate.Count == 0)
                return false;

            var postDto = new PostDto
            {
                PostStartTime = assignment.StartTime,
                PostEndTime = assignment.EndTime,
                PostLength = assignment.Duration > 0
                    ? assignment.Duration
                    : PostPositionDto.CalculateLengthOfWorkFromTimes(assignment.StartTime, assignment.EndTime) ?? 0
            };

            var otherOnNewDate = otherSlotsOnNewDate.Select(slot => new PostPositionDto
            {
                DateOfWork = slot.DateOfWork,
                PostStartTime = slot.StartTime,
                PostEndTime = slot.EndTime,
                LengthOfWork = slot.LengthOfWork > 0
                    ? slot.LengthOfWork
                    : PostPositionDto.CalculateLengthOfWorkFromTimes(slot.StartTime, slot.EndTime) ?? 0
            }).ToList();

            var (overlapMinutes, _, _, hasExactSame) = postDto.HasTimeConflictWithAlreadyTakenPostPosition(newDate, otherOnNewDate);
            return overlapMinutes > 0 || hasExactSame;
        }
    }
}

