using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing request equipment operations using the generic service pattern.
    /// </summary>
    public class RequestEquipmentService : GenericService<RequestEquipmentDto, RequestEquipment>, IRequestEquipmentService
    {
        private readonly IRequestEquipmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITokenParser _tokenParser;
        private readonly IRequestService _requestService;
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IRequestApprovalRoutingService _requestApprovalRoutingService;
        private readonly IRoutingGroupRepository _routingGroupRepository;
        private readonly IVehicleApprovalLinkBuilder _vehicleApprovalLinkBuilder;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEquipmentService"/> class.
        /// </summary>
        public RequestEquipmentService(
            IRequestEquipmentRepository repository,
            IMapper mapper,
            ILogger<RequestEquipmentService> logger,
            ITokenParser tokenParser,
            IRequestService requestService,
            IDepartmentEquipmentService departmentEquipmentService,
            IRequestApprovalRoutingService requestApprovalRoutingService,
            IRoutingGroupRepository routingGroupRepository,
            IVehicleApprovalLinkBuilder vehicleApprovalLinkBuilder,
            IServiceProvider serviceProvider) : base(repository, mapper, "RequestEquipmentID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _tokenParser = tokenParser;
            _requestService = requestService;
            _departmentEquipmentService = departmentEquipmentService;
            _requestApprovalRoutingService = requestApprovalRoutingService;
            _routingGroupRepository = routingGroupRepository;
            _vehicleApprovalLinkBuilder = vehicleApprovalLinkBuilder;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Inserts a new request equipment or updates an existing one.
        /// </summary>
        /// <param name="dto">The request equipment DTO to insert or update.</param>
        /// <returns>The upserted request equipment DTO.</returns>
        public override async Task<RequestEquipmentDto> UpsertAsync(RequestEquipmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            var isNewVehicle =
                (!dto.RequestEquipmentID.HasValue || dto.RequestEquipmentID.Value == Guid.Empty) &&
                dto.IsVehicle &&
                dto.RequestID.HasValue &&
                dto.DepartmentEquipmentID.HasValue;

            // Validate RequestID exists in database
            await ValidateForeignKeyAsync<IRequestService, RequestDto>(_requestService, dto.RequestID.Value, "Request");

            // Validate DepartmentEquipmentID exists in database
            await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(_departmentEquipmentService, dto.DepartmentEquipmentID.Value, "DepartmentEquipment");

            // Validate for duplicate RequestID + DepartmentEquipmentID combination
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.RequestID == dto.RequestID && e.DepartmentEquipmentID == dto.DepartmentEquipmentID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUEST_EQUIPMENT_ASSIGNMENT);

            // Per Job Position vehicles: always keep one unit per position.
            var requestedQuantity = dto.EquipmentQuantity;
            dto.EquipmentQuantity = EquipmentAssignmentRules.NormalizeStoredQuantity(
                dto.IsVehicle,
                EquipmentAssignmentRules.IsRequestPerJob(dto),
                dto.EquipmentQuantity);

            if (dto.IsVehicle && !EquipmentAssignmentRules.IsRequestPerJob(dto) && requestedQuantity > 1)
            {
                _logger.LogInformation(
                    "Capping Per Job Position vehicle quantity from {RequestedQuantity} to 1 for RequestID {RequestId}.",
                    requestedQuantity,
                    dto.RequestID);
            }

            // Before updating, freeze allocations for all shifts of this request that have already started or entered the 2-hour freeze window
            if (dto.RequestID.HasValue && dto.RequestID.Value != Guid.Empty)
            {
                try
                {
                    var req = await _requestService.GetByIdAsync(dto.RequestID.Value).ConfigureAwait(false);
                    if (req != null && req.DepartmentID != Guid.Empty)
                    {
                        var departmentNow = GetCurrentDepartmentDateTime();
                        var asOfTime = departmentNow.AddHours(2);
                        var empSvc = _serviceProvider.GetRequiredService<IEmployeeService>();
                        var master = await empSvc.GetMasterEmployeeByDepartmentIdAsync(req.DepartmentID).ConfigureAwait(false);
                        if (master?.EmployeeID != null && master.EmployeeID != Guid.Empty)
                        {
                            var masterDisplayName = PersonNameFormatHelper.FormatLastNameCommaFirstName(master.LastName, master.FirstName) ?? "Master System";
                            var vehicleSvc = _serviceProvider.GetRequiredService<IPostPositionVehicleService>();
                            await vehicleSvc.FinalizeStartedAllocationsForDepartmentAsync(
                                req.DepartmentID,
                                asOfTime,
                                master.EmployeeID.Value,
                                masterDisplayName).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to finalize pre-shift allocations before updating RequestEquipment for RequestID {RequestId}.", dto.RequestID);
                }
            }

            bool shouldUpsertVehicleApprovalRouting = false;
            bool shouldSendVehicleApprovalEmail = false;
            if (isNewVehicle)
            {
                var request = await _requestService.GetByIdAsync(dto.RequestID.Value);
                var existingVehicleAssignments = await _repository.GetAllAsync(
                    x => x.RequestID == dto.RequestID.Value && x.IsVehicle,
                    null,
                    null);

                var isFirstVehicleForRequest = !existingVehicleAssignments.Any();
                shouldUpsertVehicleApprovalRouting = isFirstVehicleForRequest;
                shouldSendVehicleApprovalEmail = request?.IsSpecialEvent != true && isFirstVehicleForRequest;
            }

            var result = await base.UpsertAsync(dto);

            Guid approvalId = Guid.Empty;
            if (shouldUpsertVehicleApprovalRouting)
            {
                approvalId = await UpsertVehicleApprovalRoutingAsync(result.RequestID!.Value);
            }

            if (shouldSendVehicleApprovalEmail && approvalId != Guid.Empty)
            {
                await SendVehicleApprovalEmailsAsync(result.RequestID!.Value, approvalId);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequestEquipmentWithNameDto>> GetByRequestIdWithNamesAsync(Guid requestId, PageParam? pagination = null)
        {
            var items = (await GetAllAsync(re => re.RequestID == requestId, null, pagination).ConfigureAwait(false)).ToList();
            if (items.Count == 0)
                return Array.Empty<RequestEquipmentWithNameDto>();

            var deptIds = items
                .Select(x => x.DepartmentEquipmentID.GetValueOrDefault())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var nameByDeptId = deptIds.Count > 0
                ? await _departmentEquipmentService.GetEquipmentDisplayNamesByIdsAsync(deptIds).ConfigureAwait(false)
                : new Dictionary<Guid, string?>();

            var resultRows = new List<RequestEquipmentWithNameDto>(items.Count);
            foreach (var dto in items)
            {
                var row = _mapper.Map<RequestEquipmentWithNameDto>(dto);
                var deptId = dto.DepartmentEquipmentID.GetValueOrDefault();
                if (deptId != Guid.Empty && nameByDeptId.TryGetValue(deptId, out var name))
                    row.EquipmentName = name;
                resultRows.Add(row);
            }

            return resultRows;
        }

        private async Task<Guid> UpsertVehicleApprovalRoutingAsync(Guid requestId)        {
            if (requestId == Guid.Empty)
                return Guid.Empty;

            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
                return Guid.Empty;

            var departmentId = request.DepartmentID != Guid.Empty
                ? request.DepartmentID
                : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

            if (departmentId == Guid.Empty)
                return Guid.Empty;

            var vehicleRoutingGroups = (await _routingGroupRepository.GetAllAsync(
                x => x.DepartmentID == departmentId && x.ApprovalType == ApprovalType.VehicleApproval && x.ApproverStatus == true,
                null,
                null)).ToList();

            var vehicleGroup = vehicleRoutingGroups.FirstOrDefault();
            if (vehicleGroup == null)
                return Guid.Empty;

            var existingRequestApprovalRoutings = (await _requestApprovalRoutingService.GetAllAsync(
                x => x.RequestID == requestId,
                null,
                null)).ToList();

            var existingVehicleApprovalRouting = existingRequestApprovalRoutings
                .FirstOrDefault(x => x.ApproverID == vehicleGroup.RoutingGroupId);

            if (existingVehicleApprovalRouting != null)
                return existingVehicleApprovalRouting.ApproverID;

            var nextRank = existingRequestApprovalRoutings.Any()
                ? existingRequestApprovalRoutings.Max(x => x.Rank) + 1
                : 1;

            var requestApprovalRouting = new RequestApprovalRoutingDto(
                vehicleGroup.RoutingGroupId,
                requestId,
                nextRank);
            await _requestApprovalRoutingService.UpsertAsync(requestApprovalRouting);
            return vehicleGroup.RoutingGroupId;
        }

        private async Task SendVehicleApprovalEmailsAsync(Guid requestId, Guid approvalId)
        {
            if (requestId == Guid.Empty || approvalId == Guid.Empty)
                return;

            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
                return;

            var routingGroup = (await _routingGroupRepository.GetAllAsync(
                x => x.RoutingGroupId == approvalId,
                null,
                null)).FirstOrDefault();

            if (routingGroup == null)
                return;

            var baseUrl = _vehicleApprovalLinkBuilder.GetBaseUrl();
            // Path-only URL (no query param names): /VehicleApproval/{requestId}/{routingGroupId}
            string? approvalLink = string.IsNullOrWhiteSpace(baseUrl)
                ? null
                : $"{baseUrl}/VehicleApproval/{requestId}/{approvalId}";

            var emailRequest = new SendApprovalEmailsRequestDto(
                requestId,
                approvalId,
                approvalLink);

            await _requestService.SendApprovalEmailsAsync(emailRequest, CancellationToken.None);
        }
    }
}
