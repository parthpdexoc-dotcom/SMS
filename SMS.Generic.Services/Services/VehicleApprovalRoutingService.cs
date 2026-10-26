using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service implementation for VehicleApprovalRouting operations.
    /// </summary>
    public class VehicleApprovalRoutingService : GenericService<VehicleApprovalRoutingDto, VehicleApprovalRouting>, IVehicleApprovalRoutingService
    {
        private readonly IMapper _mapper;
        private readonly IVehicleApprovalRoutingRepository _repository;
        private readonly ITokenParser? _tokenParser;
        private readonly IRoutingGroupService _routingGroupService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApprovalRoutingService"/> class.
        /// </summary>
        /// <param name="repository">The repository instance.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="routingGroupService">The routing group service for validation.</param>
        /// <param name="serviceScopeFactory">The service scope factory for resolving circular dependencies.</param>
        public VehicleApprovalRoutingService(
            IVehicleApprovalRoutingRepository repository,
            IMapper mapper,
            ITokenParser? tokenParser,
            IRoutingGroupService routingGroupService,
            IServiceScopeFactory serviceScopeFactory)
            : base(repository, mapper, "VehicleApprovalRouteID", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _tokenParser = tokenParser;
            _routingGroupService = routingGroupService ?? throw new ArgumentNullException(nameof(routingGroupService));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// Upserts multiple vehicle approval routing records using Rank-based sequencing.
        /// Handles deletion of removed records and updates ranks properly.
        /// 
        /// Behavior:
        /// - Ranks are assigned based on the order of incoming DTOs (1, 2, 3, ...)
        /// - Rank 1 is the first approver, Rank 2 is the second, etc.
        /// 
        /// Deletion handling:
        /// - If first record (Rank 1) is deleted, second record becomes Rank 1
        /// - If middle record is deleted, subsequent records' ranks are updated (shifted down)
        /// - All records not in the incoming list are deleted
        /// </summary>
        /// <param name="dtos">The collection of DTOs to upsert (order matters - first DTO gets Rank 1, second gets Rank 2, etc.).</param>
        /// <returns>A collection of upserted DTOs with updated ranks.</returns>
        public async Task<IEnumerable<VehicleApprovalRoutingDto>> BulkUpsertAsync(IEnumerable<VehicleApprovalRoutingDto> dtos, bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtosList = dtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_AT_LEAST_ONE_DTO_REQUIRED_FOR_BULK_UPSERT), nameof(dtos));

            // Validate all DTOs have the same DepartmentEquipmentID
            var departmentEquipmentId = dtosList.First().DepartmentEquipmentID;
            if (dtosList.Any(d => d.DepartmentEquipmentID != departmentEquipmentId))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ALL_DTOS_MUST_HAVE_SAME_DEPARTMENT_EQUIPMENT_ID), nameof(dtos));

            // Validate DepartmentEquipment exists (batch validation)
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var departmentEquipmentService = scope.ServiceProvider.GetRequiredService<IDepartmentEquipmentService>();
                await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(
                    departmentEquipmentService, departmentEquipmentId, "DepartmentEquipment");
            }

            // Validate all ApproverIDs (RoutingGroups) exist (batch validation)
            var approverIds = dtosList.Select(d => d.ApproverID).Distinct().ToList();
            foreach (var approverId in approverIds)
            {
                await ValidateForeignKeyAsync<IRoutingGroupService, RoutingGroupDto>(
                    _routingGroupService, approverId, "RoutingGroup");
            }

            // Get all existing records for this DepartmentEquipmentID
            var existingEntities = (await GetAllAsync(
                filter: x => x.DepartmentEquipmentID == departmentEquipmentId
            )).ToList();

            var existingDtos = _mapper.Map<List<VehicleApprovalRoutingDto>>(existingEntities);

            // Identify records to delete (existing records not in the incoming list)
            var incomingIds = dtosList
                .Where(d => d.VehicleApprovalRouteID.HasValue && d.VehicleApprovalRouteID.Value != Guid.Empty)
                .Select(d => d.VehicleApprovalRouteID.Value)
                .ToHashSet();

            var recordsToDelete = existingDtos
                .Where(e => e.VehicleApprovalRouteID.HasValue && !incomingIds.Contains(e.VehicleApprovalRouteID.Value))
                .ToList();

            // Bulk delete records that are no longer in the incoming list
            if (recordsToDelete.Any())
            {
                var idsToDelete = recordsToDelete
                    .Where(r => r.VehicleApprovalRouteID.HasValue)
                    .Select(r => r.VehicleApprovalRouteID.Value)
                    .ToList();

                await BulkDeleteAsync(idsToDelete);
            }

            // Assign ranks based on incoming order (1-based ranking)
            for (int i = 0; i < dtosList.Count; i++)
            {
                var dto = dtosList[i];
                dto.Rank = i + 1;

                // For existing records, ensure we preserve the ID
                // For new records, the ID will be generated
                if (dto.VehicleApprovalRouteID.HasValue && dto.VehicleApprovalRouteID.Value == Guid.Empty)
                {
                    dto.VehicleApprovalRouteID = null;
                }
            }

            // Bulk upsert all DTOs using base class method
            var results = await base.BulkUpsertAsync(dtosList, isSuperUser);
            return results;
        }

        /// <summary>
        /// Override GetAllAsync to return records ordered by Rank by default.
        /// </summary>
        public override async Task<IEnumerable<VehicleApprovalRoutingDto>> GetAllAsync(Expression<Func<VehicleApprovalRouting, bool>>? filter = null, Expression<Func<VehicleApprovalRouting, object>>[] includes = null, PageParam pageParam = null)
        {
            var results = await base.GetAllAsync(filter, includes, pageParam);
            // Order by Rank to ensure consistent ordering
            return results.OrderBy(r => r.Rank);
        }

        /// <summary>
        /// Override UpsertAsync to handle single record upsert with validation.
        /// </summary>
        /// <param name="dto">The DTO to upsert.</param>
        /// <returns>The upserted DTO.</returns>
        public override async Task<VehicleApprovalRoutingDto> UpsertAsync(VehicleApprovalRoutingDto dto, bool isSuperUser = false)
        {
            // Validate foreign keys
            await ValidateForeignKeyAsync<IRoutingGroupService, RoutingGroupDto>(
                _routingGroupService, dto.ApproverID, "RoutingGroup");

            // Validate DepartmentEquipment using service scope to avoid circular dependency
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var departmentEquipmentService = scope.ServiceProvider.GetRequiredService<IDepartmentEquipmentService>();
                await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(
                    departmentEquipmentService, dto.DepartmentEquipmentID, "DepartmentEquipment");
            }

            // If using Rank-based approach, validate rank uniqueness for the DepartmentEquipmentID
            if (dto.Rank > 0)
            {
                var existingRecords = (await GetAllAsync(
                    filter: x => x.DepartmentEquipmentID == dto.DepartmentEquipmentID &&
                                 x.Rank == dto.Rank &&
                                 (!dto.VehicleApprovalRouteID.HasValue || 
                                  x.VehicleApprovalRouteID != dto.VehicleApprovalRouteID.Value)
                )).ToList();

                if (existingRecords.Any())
                {
                    throw new ArgumentException($"Rank {dto.Rank} already exists for DepartmentEquipmentID {dto.DepartmentEquipmentID}. Ranks must be unique per DepartmentEquipment.");
                }
            }

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets all vehicle approval routing records by department equipment ID with routing group information.
        /// </summary>
        /// <param name="departmentEquipmentId">The department equipment ID to filter by.</param>
        /// <param name="filter">Optional additional filter expression.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A collection of vehicle approval routing response DTOs with routing group names.</returns>
        public async Task<IEnumerable<VehicleApprovalRoutingResponseDto>> GetAllVehicleRoutingGroupByDepartmentEquipmentIdAsync(Guid departmentEquipmentId, Expression<Func<VehicleApprovalRouting, bool>>? filter = null, PageParam? pagination = null)
        {
            // Combine the department equipment ID filter with any additional filter
            Expression<Func<VehicleApprovalRouting, bool>> combinedFilter = x => x.DepartmentEquipmentID == departmentEquipmentId;
            
            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(VehicleApprovalRouting), "x");
                var body = Expression.AndAlso(
                    Expression.Invoke(combinedFilter, parameter),
                    Expression.Invoke(filter, parameter)
                );
                combinedFilter = Expression.Lambda<Func<VehicleApprovalRouting, bool>>(body, parameter);
            }

            // Include the Approver navigation property to get routing group data
            Expression<Func<VehicleApprovalRouting, object>>[] includes = { x => x.Approver };

            // Get entities with includes
            var entities = (await GetAllAsync(combinedFilter, includes, pagination)).ToList();
            
            // Map to response DTOs
            var result = _mapper.Map<IEnumerable<VehicleApprovalRoutingResponseDto>>(entities);
            
            // Order by Rank to ensure consistent ordering
            return result.OrderBy(r => r.Rank);
        }
    }
}

