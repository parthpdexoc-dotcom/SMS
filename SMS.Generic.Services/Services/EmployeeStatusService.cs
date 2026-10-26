using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing employee status operations using the generic service pattern.
    /// </summary>
    public class EmployeeStatusService : GenericService<EmployeeStatusDto, EmployeeStatus>, IEmployeeStatusService
    {
        private readonly IEmployeeStatusRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusService"/> class.
        /// </summary>
        /// <param name="repository">The employee status repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public EmployeeStatusService(
            IEmployeeStatusRepository repository,
            IMapper mapper,
            ILogger<EmployeeStatusService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "EmployeeStatusID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new EmployeeStatusDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The EmployeeStatusDto to insert or update.</param>
        /// <returns>The upserted EmployeeStatusDto.</returns>
        public override async Task<EmployeeStatusDto> UpsertAsync(EmployeeStatusDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate StatusShort (globally unique)
            if (!string.IsNullOrWhiteSpace(dto.StatusShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "StatusShort",
                    value: dto.StatusShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_STATUS_SHORT);
            }

            return await base.UpsertAsync(dto);
        }
        public async Task<EmployeeStatusDto?> GetByNameAsync(string name,Guid departmentId)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return null;

            var lower = trimmed.ToLowerInvariant();
            var list = await GetAllAsync(
                s => ((s.StatusShort != null && s.StatusShort.ToLower() == lower)
                     || (s.StatusLong != null && s.StatusLong.ToLower() == lower)) && s.DepartmentID == departmentId,
                null,
                null).ConfigureAwait(false);
            return list.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task EnsureActiveStatusExistsForDepartmentAsync(Guid departmentId)
        {
            var existing = (await _repository.GetAllAsync(
                    x => x.DepartmentID == departmentId &&
                         ((x.StatusShort != null && x.StatusShort.ToLower() == "active") ||
                          (x.StatusLong != null && x.StatusLong.ToLower() == "active")),
                    null,
                    null).ConfigureAwait(false))
                .FirstOrDefault();
            if (existing != null)
                return;

            await _repository.UpsertAsync(
                new EmployeeStatus
                {
                    EmployeeStatusID = Guid.Empty,
                    DepartmentID = departmentId,
                    StatusShort = "Active",
                    StatusLong = "Active",
                    Status = true
                },
                "EmployeeStatusID").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EmployeeStatusDto>> GetActiveEmployeeStatusesForDropdownAsync(
            Guid? departmentId,
            PageParam? pagination,
            IEnumerable<Guid>? selectedIds)
        {
            var selectedIdSet = (selectedIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            // Do not OR selected IDs into this filter: GenericService.AppendDepartmentFilter ANDs token department,
            // which would drop cross-department selected rows. Active list stays department-scoped; selected rows are merged below.
            var items = (await GetAllAsync(
                x => (!departmentId.HasValue || x.DepartmentID == departmentId.Value) && x.Status == true,
                null,
                pagination).ConfigureAwait(false)).ToList();

            foreach (var sid in selectedIdSet)
            {
                if (items.Exists(x => x.EmployeeStatusID == sid))
                    continue;

                var extra = await GetByIdAsync(sid).ConfigureAwait(false);
                if (extra == null)
                    continue;

                items.Add(extra);
            }

            return items.OrderBy(x => x.StatusShort ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        public override Task<PagedList<EmployeeStatusDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<EmployeeStatus, bool>>? filter = null,
            Expression<Func<EmployeeStatus, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<EmployeeStatus, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(EmployeeStatus), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<EmployeeStatus, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(EmployeeStatus.StatusShort),
                nameof(EmployeeStatus.StatusLong)
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

