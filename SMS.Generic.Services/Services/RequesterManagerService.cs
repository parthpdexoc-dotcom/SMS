using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helper;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing RequesterManager operations using the generic service pattern.
    /// </summary>
    public class RequesterManagerService : GenericService<RequesterManagerDto, RequesterManager>, IRequesterManagerService
    {
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IRequesterService _requesterService;
        private readonly IServiceProvider _serviceProvider;

        public RequesterManagerService(
            IRequesterManagerRepository repository,
            IMapper mapper,
            ILogger<RequesterManagerService> logger,
            ITokenParser tokenParser,
            IRequesterService requesterService,
            IServiceProvider serviceProvider)
            : base(repository, mapper, "RequesterManagerID", tokenParser)
        {
            _mapper = mapper;
            _tokenParser = tokenParser;
            _requesterService = requesterService;
            _serviceProvider = serviceProvider;
        }
        public async Task<PagedList<RequestManagerResponceDto>> GetAllRequestManagerWithDetailsAsync(PageParam? pageParam, Guid? requesterId = null)
        {
            Expression<Func<RequesterManager, object>>[] includes =
            {
               x => x.Employee   
            };
            var effectivePageParam = pageParam ?? new PageParam();
            Expression<Func<RequesterManager, bool>>? filter = null;

            var adminScope = AdminRoleScopeHelper.GetScope(_tokenParser);

            if (adminScope.IsRequesterManagerOnly && adminScope.EmployeeId.HasValue)
            {
                var employeeGuid = adminScope.EmployeeId.Value;
                // Requester-manager-only users can see:
                // 1) their own manager row(s), and
                // 2) other manager rows explicitly shared via IsEmployeeShared,
                //    but only within requesters where they are configured as a manager.
                filter = rm =>
                    rm.EmployeeID == employeeGuid
                    || (rm.IsEmployeeShared
                        && rm.Requester != null
                        && rm.Requester.RequesterManagers.Any(link => link.EmployeeID == employeeGuid));
            }

            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                var rid = requesterId.Value;
                Expression<Func<RequesterManager, bool>> byRequester = rm => rm.RequesterID == rid;
                filter = filter == null ? byRequester : ExpressionCombiner.AndAlso(filter, byRequester);
            }

            var pagedManagers = await GetAllPagedAsync(filter, effectivePageParam, includes).ConfigureAwait(false);
            var details = _mapper.Map<IEnumerable<RequestManagerResponceDto>>(pagedManagers.Data);
            return new PagedList<RequestManagerResponceDto>(details, effectivePageParam, pagedManagers.TotalRecords);
        }
        public override async Task<RequesterManagerDto> UpsertAsync(RequesterManagerDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (!dto.RequesterID.HasValue)
                throw new WebApiException(System.Net.HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ID));

            if (dto.EmployeeID == Guid.Empty)
                throw new WebApiException(System.Net.HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID));

            // Validate RequesterID exists if provided
            await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterID, "Requester");

            // Validate for duplicate RequesterID + EmployeeID combination (scoped to current user)
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.RequesterID == dto.RequesterID.Value && e.EmployeeID == dto.EmployeeID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUESTER_MANAGER_ASSIGNMENT,
                isEmployeeScoped: false);

            // Set DepartmentID from token to ensure users can only work with their own department's data
            return await base.UpsertAsync(dto);
        }
        public async Task<List<Guid>> GetAssignedRequesterManagerEmployeesByRequesterId(Guid requesterId)
        {
            var requesterManagers = await GetAllAsync(
                   filter: rm => rm.RequesterID == requesterId
               );

            return requesterManagers
                .Select(rm => rm.EmployeeID)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetAvailableEmployeeOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null)
        {
            if (!requesterId.HasValue || requesterId.Value == Guid.Empty)
            {
                return Enumerable.Empty<SelectOptionDto>();
            }

            var excludeEmployeeIds = await GetAssignedRequesterManagerEmployeesByRequesterId(requesterId.Value);
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();
            return await employeeService.GetOptionsAsync(searchTerm, excludeEmployeeIds).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RequesterManagerDto?> GetByRequesterIdAndEmployeeIdAsync(Guid requesterId, Guid employeeId)
        {
            var items = await GetAllAsync(
                filter: rm => rm.RequesterID == requesterId && rm.EmployeeID == employeeId,
                includes: null,
                pageParam: null);
            return items.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Guid>> GetRequesterManagerIdsAsync(Guid requesterId, Guid employeeId)
        {
            var managers = await GetAllAsync(
                filter: rm => rm.RequesterID == requesterId && rm.EmployeeID == employeeId,
                includes: null,
                pageParam: null).ConfigureAwait(false);

            return managers
                .Where(m => m.RequesterManagerID.HasValue)
                .Select(m => m.RequesterManagerID!.Value)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Guid>> GetRequesterManagerIdsByEmployeeIdAsync(Guid employeeId)
        {
            var managers = await GetAllAsync(
                filter: rm => rm.EmployeeID == employeeId,
                includes: null,
                pageParam: null).ConfigureAwait(false);

            return managers
                .Where(m => m.RequesterManagerID.HasValue)
                .Select(m => m.RequesterManagerID!.Value)
                .Distinct()
                .ToList();
        }

        public async Task<List<Guid>> GetAssignedRequesterEmployeesByRequesterManagerId(Guid requesterManagerId)
        {
            var requesterManagers = await GetAllAsync(
                   filter: rm => rm.RequesterManagerID == requesterManagerId,
                   includes: new Expression<Func<RequesterManager, object>>[]
                   {
                       rm => rm.RequesterEmployees
                   });
            return requesterManagers
                    .SelectMany(rm => rm.RequesterEmployees
                        .Select(re => re.EmployeeID)
                        .Append(rm.EmployeeID))
                    .Distinct()
                    .ToList();
        }

        /// <inheritdoc />
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is Guid guidId)
            {
                var manager = await GetByIdAsync(guidId, rm => rm.RequesterEmployees);
                if (manager?.RequesterEmployees != null && manager.RequesterEmployees.Count > 0)
                {
                    var message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUESTER_MANAGER_HAS_EMPLOYEES_CANNOT_DELETE)
                        ?? "Cannot delete manager. First delete all employees assigned to this manager, then delete the manager.";
                    throw new WebApiException(HttpStatusCode.BadRequest, "400", message);
                }
            }
            return await base.DeleteAsync(id);
        }

        /// <inheritdoc />
        public async Task<AvailablePostReleaseTabTier> GetAvailablePostReleaseTabTierFromMappingsAsync(Guid employeeId,Guid? departmentId)
        {
            Expression<Func<RequesterManager, bool>> relevantManagersPredicate = departmentId.HasValue
                ? rm => rm.Requester != null
                        && rm.Requester.DepartmentID == departmentId.Value
                        && (rm.EmployeeID == employeeId
                            || rm.RequesterEmployees.Any(re => re.EmployeeID == employeeId))
                : rm => rm.EmployeeID == employeeId
                        || rm.RequesterEmployees.Any(re => re.EmployeeID == employeeId);

            Expression<Func<RequesterManager, object>>[] relevantIncludes = 
            {
                rm => rm.RequesterEmployees,
                rm => rm.Requester!,
            };

            var relevantManagers = (await GetAllAsync(relevantManagersPredicate, relevantIncludes, null).ConfigureAwait(false)).ToList();

            if (relevantManagers.Any(rm => rm.EmployeeID == employeeId))
                return AvailablePostReleaseTabTier.FullManagedPreferred;

            var requesterEmployeeRows = relevantManagers.SelectMany(rm => rm.RequesterEmployees.Where(re => re.EmployeeID == employeeId)).ToList();

            if (requesterEmployeeRows.Count == 0)
                return AvailablePostReleaseTabTier.RegularEmployee;

            if (requesterEmployeeRows.Any(re => re.IsPreferred))
                return AvailablePostReleaseTabTier.FullManagedPreferred;

            return AvailablePostReleaseTabTier.RequesterEmployeeNonPreferred;
        }

        /// <inheritdoc />
        public async Task<AvailablePostReleaseTabTier> GetAvailablePostReleaseTabTierForLoginAsync(
            EmployeeLoginInfoDto employeeInfo)
        {
            if (TokenHelper.EmployeeHasDepartmentAdminPolicyScope(employeeInfo.AdminTypes))
                return AvailablePostReleaseTabTier.FullManagedPreferred;

            return await GetAvailablePostReleaseTabTierFromMappingsAsync(employeeInfo.EmployeeID,employeeInfo.DepartmentID).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null,
            Guid? selectedId = null)
        {
            Expression<Func<RequesterManager, bool>>? filter = null;

            if (selectedId.HasValue && selectedId.Value != Guid.Empty)
            {
                var sid = selectedId.Value;
                filter = rm => rm.RequesterManagerID == sid;
            }

            var adminScope = AdminRoleScopeHelper.GetScope(_tokenParser);

            if (adminScope.IsRequesterManagerOnly && adminScope.EmployeeId.HasValue)
            {
                var employeeGuid = adminScope.EmployeeId.Value;
                Expression<Func<RequesterManager, bool>> rmFilter = rm =>
                    rm.EmployeeID == employeeGuid
                    || (rm.IsEmployeeShared
                        && rm.Requester != null
                        && rm.Requester.RequesterManagers.Any(link => link.EmployeeID == employeeGuid));
                filter = filter == null ? rmFilter : ExpressionCombiner.AndAlso(filter, rmFilter);
            }

            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                var rid = requesterId.Value;
                Expression<Func<RequesterManager, bool>> byRequester = rm => rm.RequesterID == rid;
                filter = filter == null ? byRequester : ExpressionCombiner.AndAlso(filter, byRequester);
            }

            // Nested Employee name search (generic options search only supports top-level string props).
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                Expression<Func<RequesterManager, bool>> byName = rm =>
                    (rm.Employee!.FirstName != null && rm.Employee.FirstName.ToLower().Contains(term))
                    || (rm.Employee!.LastName != null && rm.Employee.LastName.ToLower().Contains(term));
                filter = filter == null ? byName : ExpressionCombiner.AndAlso(filter, byName);
            }

            return await GetOptionsAsync(
                rm => new SelectOptionDto
                {
                    Id = rm.RequesterManagerID,
                    Label = (rm.Employee!.LastName ?? string.Empty) + ", " + (rm.Employee!.FirstName ?? string.Empty)
                },
                filter,
                searchTerm: null).ConfigureAwait(false);
        }

    }
}


