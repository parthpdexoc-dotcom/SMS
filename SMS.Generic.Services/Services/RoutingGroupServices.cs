using AutoMapper;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    public class RoutingGroupServices : GenericService<RoutingGroupDto, RoutingGroup>, IRoutingGroupService
    {
        private readonly IMapper _mapper;
        private readonly IRoutingGroupRepository _repository;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenParser? _tokenParser;

        public RoutingGroupServices(
            IRoutingGroupRepository repository, 
            IMapper mapper, 
            IEmployeeService employeeService,
            ITokenParser? tokenParser = null) 
            : base(repository, mapper, "RoutingGroupId", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _employeeService = employeeService;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Gets routing groups by department ID (from token) and status (active = 1).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of routing groups filtered by department ID and active status.</returns>
        public async Task<IEnumerable<RoutingGroupDto>> GetByDepartmentAndStatusAsync(PageParam? pagination = null)
        {
            var tokenDepartmentId = _tokenParser?.GetDepartmentID();
            if (!tokenDepartmentId.HasValue || tokenDepartmentId.Value == Guid.Empty)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

            var departmentId = tokenDepartmentId.Value;
            Expression<Func<RoutingGroup, bool>> filter = x => x.DepartmentID == departmentId && x.ApproverStatus == true;
            return await base.GetAllAsync(filter, null, pagination);
        }

        public override async Task<RoutingGroupDto> GetByIdAsync(Guid id)
        {
            var tokenDepartmentId = _tokenParser?.GetDepartmentID();
            if (!tokenDepartmentId.HasValue || tokenDepartmentId.Value == Guid.Empty)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

            var result = await base.GetByIdAsync(id);
            if (result != null && result.DepartmentID != tokenDepartmentId.Value)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ROUTING_GROUP_ACCESS_NOT_ALLOWED));

            return result;
        }

        public override async Task<RoutingGroupDto> UpsertAsync(RoutingGroupDto dto, bool isSuperUser = false)
        {
            var tokenDepartmentId = _tokenParser?.GetDepartmentID();
            if (!tokenDepartmentId.HasValue || tokenDepartmentId.Value == Guid.Empty)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

            dto.DepartmentID = tokenDepartmentId.Value;

            if (dto.RoutingGroupId.HasValue && dto.RoutingGroupId.Value != Guid.Empty)
            {
                var existing = await GetByIdAsync(dto.RoutingGroupId.Value);
                if (existing != null && existing.DepartmentID != dto.DepartmentID)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ROUTING_GROUP_EDIT_NOT_ALLOWED));

                if (existing != null && !HasChanges(dto, existing))
                    return existing;
            }

            if (dto.EmployeeIDs?.Any() == true)
            {
                var employeeIds = dto.EmployeeIDs.Where(id => id != Guid.Empty).Distinct().ToList();
                if (employeeIds.Any())
                {
                    var existingIds = (await _employeeService.GetAllAsync(e => employeeIds.Contains(e.EmployeeID)))
                        .Select(e => e.EmployeeID).ToHashSet();
                    var missing = employeeIds.Where(id => !existingIds.Contains(id)).ToList();
                    if (missing.Any())
                    {
                        var message = missing.Count == 1
                            ? $"Referenced Employee with ID {missing[0]} does not exist."
                            : $"Referenced Employees with IDs {string.Join(", ", missing)} do not exist.";
                        throw new ArgumentException(message);
                    }
                }
            }

            Guid currentRoutingGroupId = dto.RoutingGroupId.GetValueOrDefault();
            bool isUpdate = currentRoutingGroupId != Guid.Empty;

            // Each department can have only one routing group per approval type.
            // When updating, exclude the current record so non-type changes do not fail duplicate validation.
            var sameTypeInDept = (await _repository.GetAllAsync(
                x => x.DepartmentID == dto.DepartmentID &&
                     x.ApprovalType == dto.ApprovalType &&
                     (!isUpdate || x.RoutingGroupId != currentRoutingGroupId),
                null,
                null)).ToList();

            if (sameTypeInDept.Any())
                throw new ArgumentException(string.Format(ModelValidationErrorCodes.ROUTING_GROUP_APPROVAL_TYPE_ALREADY_EXISTS, dto.ApprovalType));

            return await base.UpsertAsync(dto);
        }

        private bool HasChanges(RoutingGroupDto newDto, RoutingGroupDto existingDto)
        {
            if (newDto.ApproverLong != existingDto.ApproverLong ||
                newDto.ApproverShort != existingDto.ApproverShort ||
                newDto.ApproverEmail != existingDto.ApproverEmail ||
                newDto.ApprovalType != existingDto.ApprovalType ||
                newDto.ApproverStatus != existingDto.ApproverStatus)
                return true;

            var newIds = newDto.EmployeeIDs?.Where(id => id != Guid.Empty).OrderBy(id => id).ToList() ?? new List<Guid>();
            var existingIds = existingDto.EmployeeIDs?.Where(id => id != Guid.Empty).OrderBy(id => id).ToList() ?? new List<Guid>();
            return !newIds.SequenceEqual(existingIds);
        }

        /// <summary>
        /// Gets all routing groups with employee first names resolved from EmployeeIDs via employee service.
        /// </summary>
        public async Task<PagedList<RoutingGroupDetailsDto>> GetAllRoutingGroupDetailsAsync(PageParam? pagination = null)
        {
            var pagedRoutingGroups = await GetAllPagedAsync(null, pagination).ConfigureAwait(false);
            var routingGroups = pagedRoutingGroups.Data.ToList();

            var employeeIds = routingGroups
                .SelectMany(rg => rg.EmployeeIDs ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var employeeDict = new Dictionary<Guid, string>();
            if (employeeIds.Any())
            {
                var employees = await _employeeService.GetAllAsync(e => employeeIds.Contains(e.EmployeeID));
                foreach (var e in employees)
                    if (e?.EmployeeID != null)
                        employeeDict[e.EmployeeID.Value] = (e.FirstName ?? "").Trim();
            }

            var dtos = _mapper.Map<List<RoutingGroupDetailsDto>>(routingGroups);
            foreach (var dto in dtos)
            {
                dto.EmployeeFirstNames = dto.EmployeeIDs?
                    .Select(id => employeeDict.GetValueOrDefault(id))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();
            }

            return new PagedList<RoutingGroupDetailsDto>(dtos, pagedRoutingGroups, pagedRoutingGroups.TotalRecords);
        }

        public override Task<PagedList<RoutingGroupDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<RoutingGroup, bool>>? filter = null,
            Expression<Func<RoutingGroup, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<RoutingGroup, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(RoutingGroup), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<RoutingGroup, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(RoutingGroup.ApproverShort),
                nameof(RoutingGroup.ApproverLong),
                nameof(RoutingGroup.ApproverEmail)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                baseFilter,
                includes,
                effectiveSearchableProperties
            );
        }
    }
}
