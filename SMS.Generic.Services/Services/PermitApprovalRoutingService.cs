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
    /// Service implementation for PermitApprovalRouting operations.
    /// </summary>
    public class PermitApprovalRoutingService : GenericService<PermitApprovalRoutingDto, PermitApprovalRouting>, IPermitApprovalRoutingService
    {
        private readonly IMapper _mapper;
        private readonly IPermitApprovalRoutingRepository _repository;
        private readonly ITokenParser? _tokenParser;
        private readonly IRoutingGroupService _routingGroupService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermitApprovalRoutingService"/> class.
        /// </summary>
        public PermitApprovalRoutingService(
            IPermitApprovalRoutingRepository repository,
            IMapper mapper,
            ITokenParser? tokenParser,
            IRoutingGroupService routingGroupService,
            IServiceScopeFactory serviceScopeFactory)
            : base(repository, mapper, "PermitApprovalRoutingID", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _tokenParser = tokenParser;
            _routingGroupService = routingGroupService ?? throw new ArgumentNullException(nameof(routingGroupService));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// Override UpsertAsync to validate Request and RoutingGroup foreign keys.
        /// </summary>
        public override async Task<PermitApprovalRoutingDto> UpsertAsync(PermitApprovalRoutingDto dto, bool isSuperUser = false)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                await ValidateForeignKeyAsync<IRequestService, RequestDto>(requestService, dto.RequestID, "Request");
            }

            await ValidateForeignKeyAsync<IRoutingGroupService, RoutingGroupDto>(
                _routingGroupService, dto.RoutingGroupId, "RoutingGroup");

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Bulk upsert for permit approval routing: same logic as RequestApprovalRouting.
        /// - All DTOs must have the same RequestID.
        /// - Records with PermitApprovalRoutingID are updated; records without (or with empty GUID) are inserted.
        /// - Existing records for this RequestID that are not in the incoming list are deleted.
        /// </summary>
        public async Task<IEnumerable<PermitApprovalRoutingDto>> BulkUpsertByRequestIdAsync(IEnumerable<PermitApprovalRoutingDto> dtos)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtosList = dtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_AT_LEAST_ONE_DTO_REQUIRED_FOR_BULK_UPSERT), nameof(dtos));

            // Validate all DTOs have the same RequestID
            var requestId = dtosList.First().RequestID;
            if (dtosList.Any(d => d.RequestID != requestId))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ALL_DTOS_MUST_HAVE_SAME_REQUEST_ID), nameof(dtos));

            // Validate Request exists (batch validation)
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                await ValidateForeignKeyAsync<IRequestService, RequestDto>(
                    requestService, requestId, "Request");
            }

            // Validate all RoutingGroupIds exist (batch validation)
            var routingGroupIds = dtosList.Select(d => d.RoutingGroupId).Distinct().ToList();
            foreach (var routingGroupId in routingGroupIds)
            {
                await ValidateForeignKeyAsync<IRoutingGroupService, RoutingGroupDto>(
                    _routingGroupService, routingGroupId, "RoutingGroup");
            }

            // Get all existing records for this RequestID
            var existingEntities = (await GetAllAsync(
                filter: x => x.RequestID == requestId
            )).ToList();

            var existingDtos = _mapper.Map<List<PermitApprovalRoutingDto>>(existingEntities);

            // Identify records to delete (existing records not in the incoming list)
            var incomingIds = dtosList
                .Where(d => d.PermitApprovalRoutingID.HasValue && d.PermitApprovalRoutingID.Value != Guid.Empty)
                .Select(d => d.PermitApprovalRoutingID.Value)
                .ToHashSet();

            var recordsToDelete = existingDtos
                .Where(e => e.PermitApprovalRoutingID.HasValue && !incomingIds.Contains(e.PermitApprovalRoutingID.Value))
                .ToList();

            // Bulk delete records that are no longer in the incoming list
            if (recordsToDelete.Any())
            {
                var idsToDelete = recordsToDelete
                    .Where(r => r.PermitApprovalRoutingID.HasValue)
                    .Select(r => r.PermitApprovalRoutingID!.Value)
                    .ToList();

                await BulkDeleteAsync(idsToDelete);
            }

            // For existing records preserve the ID; for new records the ID will be generated
            foreach (var dto in dtosList)
            {
                if (dto.PermitApprovalRoutingID.HasValue && dto.PermitApprovalRoutingID.Value == Guid.Empty)
                {
                    dto.PermitApprovalRoutingID = null;
                }
            }

            // Bulk upsert all DTOs using base class method
            var results = await base.BulkUpsertAsync(dtosList, isSuperUser: false);
            return results;
        }

        /// <summary>
        /// Gets all permit approval routing records by request ID with routing group information.
        /// </summary>
        public async Task<IEnumerable<PermitApprovalRoutingResponseDto>> GetAllByRequestIdAsync(Guid requestId, Expression<Func<PermitApprovalRouting, bool>>? filter = null, PageParam? pagination = null)
        {
            Expression<Func<PermitApprovalRouting, bool>> combinedFilter = x => x.RequestID == requestId;
            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(PermitApprovalRouting), "x");
                var body = Expression.AndAlso(
                    Expression.Invoke(combinedFilter, parameter),
                    Expression.Invoke(filter, parameter)
                );
                combinedFilter = Expression.Lambda<Func<PermitApprovalRouting, bool>>(body, parameter);
            }

            Expression<Func<PermitApprovalRouting, object>>[] includes = { x => x.Approver };
            var entities = (await GetAllAsync(combinedFilter, includes, pagination)).ToList();
            return _mapper.Map<IEnumerable<PermitApprovalRoutingResponseDto>>(entities);
        }
    }
}
