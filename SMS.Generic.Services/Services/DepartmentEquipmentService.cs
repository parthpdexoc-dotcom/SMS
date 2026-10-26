using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for DepartmentEquipment operations.
    /// </summary>
    public class DepartmentEquipmentService : GenericService<DepartmentEquipmentDto, DepartmentEquipment>, IDepartmentEquipmentService
    {
        private readonly ITokenParser _tokenParser;
        private readonly IEquipmentTypeService _equipmentTypeService;
        private readonly IVehicleApprovalRoutingService _vehicleApprovalRoutingService;
        private readonly IRoutingGroupService _routingGroupService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IDepartmentEquipmentRepository _repository;
        public DepartmentEquipmentService(
            IDepartmentEquipmentRepository repository,
            IMapper mapper,
            ILogger<DepartmentEquipmentService> logger,
            ITokenParser tokenParser,
            IEquipmentTypeService equipmentTypeService,
            IVehicleApprovalRoutingService vehicleApprovalRoutingService,
            IRoutingGroupService routingGroupService)
            : base(repository, mapper, "DepartmentEquipmentID", tokenParser)
        {
            _tokenParser = tokenParser;
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _equipmentTypeService = equipmentTypeService;
            _vehicleApprovalRoutingService = vehicleApprovalRoutingService ?? throw new ArgumentNullException(nameof(vehicleApprovalRoutingService));
            _routingGroupService = routingGroupService ?? throw new ArgumentNullException(nameof(routingGroupService));
        }

        /// <summary>
        /// Inserts a new department equipment or updates an existing one.
        /// </summary>
        /// <param name="dto">The department equipment DTO to insert or update.</param>
        /// <returns>The upserted department equipment DTO.</returns>
        public override async Task<DepartmentEquipmentDto> UpsertAsync(DepartmentEquipmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate EquipmentTypeID exists in database
            await ValidateForeignKeyAsync<IEquipmentTypeService, EquipmentTypeDto>(_equipmentTypeService, dto.EquipmentTypeID, "EquipmentType");

            // Get EquipmentType to check if it's a vehicle
            var equipmentType = await _equipmentTypeService.GetByIdAsync(dto.EquipmentTypeID);

            // Validate for duplicate DepartmentID + EquipmentTypeID combination
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.DepartmentID == dto.DepartmentID && e.EquipmentTypeID == dto.EquipmentTypeID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_DEPARTMENT_EQUIPMENT_TYPE);

            // Upsert the DepartmentEquipment first
            var result = await base.UpsertAsync(dto);

            // Handle VehicleApprovalRouting if equipment is a vehicle
            if (equipmentType?.IsVehicle == true && result.DepartmentEquipmentID.HasValue)
            {
                await HandleVehicleApprovalRoutingAsync(dto, result);
            }

            return result;
        }

        /// <summary>
        /// Deletes all existing VehicleApprovalRouting records for a DepartmentEquipmentID.
        /// </summary>
        private async Task DeleteExistingVehicleApprovalRoutingAsync(Guid departmentEquipmentID)
        {
            var existingRoutingRecords = await _vehicleApprovalRoutingService.GetAllAsync(
                filter: x => x.DepartmentEquipmentID == departmentEquipmentID
            );

            foreach (var record in existingRoutingRecords)
            {
                if (record.VehicleApprovalRouteID.HasValue)
                {
                    await _vehicleApprovalRoutingService.DeleteAsync<Guid>(record.VehicleApprovalRouteID.Value);
                }
            }
        }

        /// <summary>
        /// Handles vehicle approval routing for DepartmentEquipment using RoutingGroup with ApprovalType.VehicleApproval.
        /// Mirrors the special event approval handling used for requests.
        /// </summary>
        private async Task HandleVehicleApprovalRoutingAsync(DepartmentEquipmentDto dto, DepartmentEquipmentDto result)
        {
            if (!result.DepartmentEquipmentID.HasValue)
            {
                return;
            }

            if (dto.RequiresApproval == true)
            {
                // Determine department ID from the result or token context
                var departmentId = result.DepartmentID != Guid.Empty
                    ? result.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                if (departmentId == Guid.Empty)
                {
                    return;
                }

                // Get the routing group for VehicleApproval for this department
                var vehicleRoutingGroups = (await _routingGroupService.GetAllAsync(
                    x => x.DepartmentID == departmentId && x.ApprovalType == ApprovalType.VehicleApproval,
                    null,
                    null)).ToList();

                var vehicleGroup = vehicleRoutingGroups.FirstOrDefault();
                var vehicleRoutingGroupId = vehicleGroup?.RoutingGroupId ?? Guid.Empty;

                if (vehicleGroup == null || vehicleRoutingGroupId == Guid.Empty)
                {
                    // No routing group configured for vehicle approval; nothing to do
                    return;
                }

                var vehicleApprovalRouting = new VehicleApprovalRoutingDto
                {
                    DepartmentEquipmentID = result.DepartmentEquipmentID.Value,
                    ApproverID = vehicleRoutingGroupId,
                    Rank = 1,
                    VehicleApproverHasFinalAuthority = true
                };

                await _vehicleApprovalRoutingService.UpsertAsync(vehicleApprovalRouting);
            }
            else
            {
                // If approval is not required, delete all existing routing records
                await DeleteExistingVehicleApprovalRoutingAsync(result.DepartmentEquipmentID.Value);
            }
        }

        public async Task<IEnumerable<DepartmentEquipmentResponseDto>> GetDepartmentEquipmentByTypeAsync(Guid departmentId, EquipmentCategory category, IEnumerable<Guid>? selectedIds = null)
        {
            bool isVehicle = category == EquipmentCategory.Vehicle;
            var selectedIdList = (selectedIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            Expression<Func<DepartmentEquipment, object>>[] includes ={ x => x.EquipmentType };

            var departmentEquipment = (await GetAllAsync(
                filter: x => x.DepartmentID == departmentId
                             && x.EquipmentType != null
                             && x.EquipmentType.IsVehicle == isVehicle
                             && ((x.EquipmentStatus == true && x.EquipmentType.EquipementTypeStatus == true)
                                 || selectedIdList.Contains(x.DepartmentEquipmentID)),
                includes: includes
            )).ToList();

            // Map and ensure dropdown values are sorted A-Z by equipment type display name
            var mapped = _mapper.Map<List<DepartmentEquipmentResponseDto>>(departmentEquipment);
            return mapped.OrderBy(de => de.EquipmentType);
        }
        public Task<List<string>> GetVehicleNamesByDepartmentEquipmentIdsAsync(IEnumerable<Guid> ids)
        {
           return _repository.GetVehicleNamesByDepartmentEquipmentIdsAsync(ids);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<Guid, string?>> GetEquipmentDisplayNamesByIdsAsync(IEnumerable<Guid> departmentEquipmentIds)
        {
            var ids = (departmentEquipmentIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
            if (ids.Count == 0)
                return new Dictionary<Guid, string?>();

            var items = (await GetAllAsync(
                de => ids.Contains(de.DepartmentEquipmentID),
                new Expression<Func<DepartmentEquipment, object>>[] { x => x.EquipmentType! },
                pageParam: null)).ToList();

            var dict = new Dictionary<Guid, string?>(ids.Count);
            foreach (var id in ids)
                dict[id] = null;

            foreach (var dto in items)
            {
                if (!dto.DepartmentEquipmentID.HasValue || dto.DepartmentEquipmentID.Value == Guid.Empty)
                    continue;
                dict[dto.DepartmentEquipmentID.Value] = ResolveEquipmentTypeDisplayName(dto.EquipmentType);
            }

            return dict;
        }

        private static string? ResolveEquipmentTypeDisplayName(EquipmentType? equipmentType)
        {
            if (equipmentType == null)
                return null;
            var shortName = equipmentType.EquipementTypeShort?.Trim();
            var longName = equipmentType.EquipementTypeLong?.Trim();
            return !string.IsNullOrEmpty(shortName) ? shortName : longName;
        }

        /// <inheritdoc />
        public Task<IEnumerable<DepartmentEquipmentSelectOptionDto>> GetDepartmentVehicleOptionsAsync(
            IEnumerable<Guid>? selectedIds = null,
            string? searchTerm = null) =>
            GetOptionsByVehicleCategoryAsync(isVehicle: true, selectedIds, searchTerm);

        /// <inheritdoc />
        public Task<IEnumerable<DepartmentEquipmentSelectOptionDto>> GetDepartmentEquipmentOptionsAsync(
            IEnumerable<Guid>? selectedIds = null,
            string? searchTerm = null) =>
            GetOptionsByVehicleCategoryAsync(isVehicle: false, selectedIds, searchTerm);

        private async Task<IEnumerable<DepartmentEquipmentSelectOptionDto>> GetOptionsByVehicleCategoryAsync(
            bool isVehicle,
            IEnumerable<Guid>? selectedIds,
            string? searchTerm)
        {

            var selectedIdList = (selectedIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var trimmedSearch = searchTerm?.Trim();
            var hasSearch = !string.IsNullOrWhiteSpace(trimmedSearch);
            var loweredSearch = hasSearch ? trimmedSearch!.ToLower() : null;

            Expression<Func<DepartmentEquipment, object>>[] includes = { x => x.EquipmentType! };

            var items = (await GetAllAsync(
                filter: x => x.EquipmentType != null
                    && x.EquipmentType.IsVehicle == isVehicle
                    && ((x.EquipmentStatus == true && x.EquipmentType.EquipementTypeStatus == true)
                        || selectedIdList.Contains(x.DepartmentEquipmentID))
                    && (!hasSearch
                        || (x.EquipmentType.EquipementTypeShort != null
                            && x.EquipmentType.EquipementTypeShort.ToLower().Contains(loweredSearch!))
                        || (x.EquipmentType.EquipementTypeLong != null
                            && x.EquipmentType.EquipementTypeLong.ToLower().Contains(loweredSearch!))),
                includes: includes)).ToList();

            return items
                .Where(e => e.DepartmentEquipmentID.HasValue && e.DepartmentEquipmentID.Value != Guid.Empty)
                .Select(e => new DepartmentEquipmentSelectOptionDto
                {
                    Id = e.DepartmentEquipmentID!.Value,
                    Label = e.EquipmentType != null
                        ? (e.EquipmentType.EquipementTypeShort ?? e.EquipmentType.EquipementTypeLong ?? string.Empty)
                        : string.Empty,
                    IsDefault = e.IsDefault,
                })
                .OrderByDescending(o => o.IsDefault)
                .ThenBy(o => o.Label, StringComparer.OrdinalIgnoreCase);
        }

        public override Task<PagedList<DepartmentEquipmentDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentEquipment, bool>>? filter = null,
            Expression<Func<DepartmentEquipment, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            var defaultIncludes = includes ?? new Expression<Func<DepartmentEquipment, object>>[] { x => x.EquipmentType };
            var defaultFilter = filter ?? (x => x.EquipmentStatus == true);

            return SearchPagedAsync(
                searchTerm,
                defaultFilter,
                term =>
                {
                    var lowered = term.Trim().ToLower();
                    return x =>
                        x.EquipmentType != null &&
                        (
                            (x.EquipmentType.EquipementTypeShort != null &&
                             x.EquipmentType.EquipementTypeShort.ToLower().Contains(lowered))
                            ||
                            (x.EquipmentType.EquipementTypeLong != null &&
                             x.EquipmentType.EquipementTypeLong.ToLower().Contains(lowered))
                        );
                },
                pageParam,
                defaultIncludes);
        }
    }
}


