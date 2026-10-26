using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing approval detail operations (ApprovalDetails table) using the generic service pattern.
    /// </summary>
    public class ApprovalDetailService : GenericService<ApprovalDetailDto, ApprovalDetail>, IApprovalDetailService
    {
        private readonly IApprovalDetailRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IDepartmentService _departmentService;
        private readonly IRequestService _requestService;
        private readonly IRequesterService _requesterService;
        private readonly IPostService _postService;
        private readonly IRequestEquipmentRepository _requestEquipmentRepository;
        private readonly IRequestApprovalRoutingService _requestApprovalRoutingService;
        private readonly IEquipmentTypeService _equipmentTypeService;
        private readonly IRoutingGroupService _routingGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalDetailService"/> class.
        /// </summary>
        public ApprovalDetailService(
            IApprovalDetailRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<ApprovalDetailService> logger,
            IDepartmentService departmentService,
            IRequestService requestService,
            IRequesterService requesterService,
            IPostService postService,
            IRequestEquipmentRepository requestEquipmentRepository,
            IRequestApprovalRoutingService requestApprovalRoutingService,
            IEquipmentTypeService equipmentTypeService,
            IRoutingGroupService routingGroupService)
            : base(repository, mapper, "ApprovalDetailID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _departmentService = departmentService;
            _requestService = requestService;
            _requesterService = requesterService;
            _postService = postService;
            _requestEquipmentRepository = requestEquipmentRepository;
            _requestApprovalRoutingService = requestApprovalRoutingService;
            _equipmentTypeService = equipmentTypeService;
            _routingGroupService = routingGroupService;
        }

        /// <summary>
        /// Inserts a new approval detail or updates an existing one with basic validation.
        /// </summary>
        public override async Task<ApprovalDetailDto> UpsertAsync(ApprovalDetailDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            await ValidateForeignKeyAsync<IRequestService, RequestDto>(_requestService, dto.RequestID, "Request");

            return await base.UpsertAsync(dto, isSuperUser);
        }

        /// <inheritdoc />
        public async Task<ApprovalRequestDetailResponseDto?> GetApprovalRequestDetailAsync(Guid requestId, Guid routingGroupId, ApprovalType? approvalType, CancellationToken cancellationToken = default)
        {
            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
                return null;

            var routingList = (await _requestApprovalRoutingService.GetAllAsync(
                x => x.RequestID == requestId,
                null,
                null)).ToList();

            if (routingList.All(r => r.ApproverID != routingGroupId))
            {
                throw new WebApiException(
                    HttpStatusCode.Forbidden,
                    ((int)HttpStatusCode.Forbidden).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.VEHICLE_APPROVAL_ROUTING_GROUP_NOT_ASSIGNED));
            }

            var employeeId = _tokenParser.GetEmployeeID();
            if (!employeeId.HasValue || employeeId.Value == Guid.Empty)
                throw new WebApiException(HttpStatusCode.Forbidden, ((int)HttpStatusCode.Forbidden).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.VEHICLE_APPROVAL_EMPLOYEE_ID_MISSING));

            var routingGroup = await _routingGroupService.GetByIdAsync(routingGroupId);
            if (routingGroup?.EmployeeIDs == null || !routingGroup.EmployeeIDs.Contains(employeeId.Value))
                throw new WebApiException(HttpStatusCode.Forbidden, ((int)HttpStatusCode.Forbidden).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.VEHICLE_APPROVAL_NOT_MEMBER_OF_GROUP));

            RequesterDto? requester = null;
            if (request.RequesterID.HasValue && request.RequesterID.Value != Guid.Empty)
                requester = await _requesterService.GetByIdAsync(request.RequesterID.Value);

            var contact = string.Concat(requester?.PrimaryContact?.Trim(), " - ", requester?.Phone?.Trim()).Trim('-', ' ');
            var requesterInfo = new RequesterInfoDto(requester?.Name, string.IsNullOrEmpty(contact) ? null : contact, request.PrimaryEmail ?? requester?.Email);

            int vehiclesRequired = 0;
            var vehicleTypeNames = new List<string>();
            var vehicleEquipments = await _requestEquipmentRepository.GetAllAsync(
                x => x.RequestID == requestId,
                new Expression<Func<RequestEquipment, object>>[] { x => x.DepartmentEquipment! },
                null);
            var vehicleList = vehicleEquipments?.ToList() ?? new List<RequestEquipment>();
            foreach (var ve in vehicleList)
            {
                vehiclesRequired += ve.EquipmentQuantity;
                if (ve.DepartmentEquipment != null)
                {
                    var eqType = (await _equipmentTypeService.GetAllAsync(x => x.EquipementTypeID == ve.DepartmentEquipment.EquipmentTypeID && x.IsVehicle == true)).FirstOrDefault();
                    var name = eqType?.EquipementTypeShort ?? eqType?.EquipementTypeLong;
                    if (!string.IsNullOrWhiteSpace(name) && !vehicleTypeNames.Contains(name))
                        vehicleTypeNames.Add(name);
                }
            }

            var requestedDate = request.StartDate != default ? request.StartDate.ToString("MMMM d, yyyy") : null;
            var vehicleType = vehicleTypeNames.Count > 0 ? string.Join(", ", vehicleTypeNames) : null;
            var requestVehicleInfo = new RequestVehicleInfoDto(requestedDate, vehiclesRequired, vehicleType);

            var postPaged = await _postService.GetPostByRequestIdAsync(requestId, null);
            var posts = postPaged?.Data?.ToList() ?? new List<PostDto>();
            var postDetails = posts.Select(p => new PostDetailDto(
                p.PostStartTime != default ? p.PostStartTime.ToString("h:mm tt") : null,
                p.PostLocation,
                p.PostTitle)).ToList();

            // Load existing approval details for this request.
            var approvalDetails = await _repository.GetAllAsync(
                x => x.RequestID == requestId,
                null,
                null);
            var approvalDetailsList = approvalDetails?.ToList() ?? new List<ApprovalDetail>();

            List<ApprovalRoutingStatusDto> approvalRouting = routingList
                .Select(r =>
                {
                    var detail = approvalDetailsList.FirstOrDefault(d =>
                        d.RouteToID == r.ApproverID);

                    bool? isApproved = null;
                    bool? finalApproval = null;

                    if (detail != null)
                    {
                        isApproved = detail.Approved;
                        finalApproval = detail.FinalApproval;
                    }

                    return new ApprovalRoutingStatusDto(
                        r.ApproverID,
                        r.Rank,
                        isApproved,
                        finalApproval);
                })
                .OrderBy(r => r.Rank)
                .ToList();

            // When approvalType is provided, only consider an existing detail that matches that type.
            var existingDetailForCurrentGroup = approvalType.HasValue
                ? approvalDetailsList.FirstOrDefault(d => d.RouteToID == routingGroupId && d.Type == approvalType.Value)
                : approvalDetailsList.FirstOrDefault(d => d.RouteToID == routingGroupId);
            var alreadySubmittedByCurrentApprover = existingDetailForCurrentGroup != null;

            var result = new ApprovalRequestDetailResponseDto(
                requestId,
                routingGroupId,
                requesterInfo,
                requestVehicleInfo,
                postDetails,
                approvalRouting);
            result.AlreadySubmittedByCurrentApprover = alreadySubmittedByCurrentApprover;
            return result;
        }
    }
}
