using AutoMapper;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Common.Implementations.Paginations;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department divisions operations using the generic service pattern.
    /// </summary>
    public class DepartmentDivisionsService : GenericService<DepartmentDivisionsDto, DepartmentDivisions>, IDepartmentDivisionsService
    {
        private readonly IDepartmentDivisionsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsService"/> class.
        /// </summary>
        /// <param name="repository">The department divisions repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentDivisionsService(
            IDepartmentDivisionsRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<DepartmentDivisionsService> logger) : base(repository, mapper, "DivisionID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }
        /// <summary>
        /// Inserts a new department division or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The department division DTO to insert or update.</param>
        /// <returns>The upserted department division DTO.</returns>
        public override async Task<DepartmentDivisionsDto> UpsertAsync(DepartmentDivisionsDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate DivisionShort within the same department.
            // DepartmentID can come from the request (dto) or from the token when not provided.
            if (!string.IsNullOrWhiteSpace(dto.DivisionShort))
            {
                if (dto.DepartmentID != Guid.Empty)
                {
                    // Department from request: scope duplicate check by dto.DepartmentID
                    var departmentId = dto.DepartmentID;
                    await ValidateDuplicateAsync(
                        propertyName: "DivisionShort",
                        value: dto.DivisionShort,
                        dto: dto,
                        isDepartmentScoped: false,
                        additionalFilter: d => d.DepartmentID == departmentId,
                        errorMessage: ModelValidationErrorCodes.DUPLICATE_DIVISION_SHORT);
                }
                else
                {
                    // Department from token: use built-in department scoping
                    await ValidateDuplicateAsync(
                        propertyName: "DivisionShort",
                        value: dto.DivisionShort,
                        dto: dto,
                        isDepartmentScoped: true,
                        errorMessage: ModelValidationErrorCodes.DUPLICATE_DIVISION_SHORT);
                }
            }

            // DepartmentDivisions audit FKs point to Employee table.
            // For admin-token requests, GenericService audit mapping sets AdminID into CreatedUserID/EditedUserID,
            // which violates FK_DepartmentDivisions_Employee_*. Bypass automatic audit mapping for admin users here.
            var isAdminUser = Enum.TryParse<UserType>(_tokenParser.GetClaimValue(ClaimNames.UserType), true, out var userType)
                              && userType == UserType.Admin;

            if (dto.CreatedUserID == Guid.Empty)
                dto.CreatedUserID = null;
            if (dto.EditedUserID == Guid.Empty)
                dto.EditedUserID = null;

            return await base.UpsertAsync(dto, isSuperUser || isAdminUser);
        }

        /// <inheritdoc />
        public async Task<DepartmentDivisionsDto?> GetByDepartmentIdAndNameAsync(Guid departmentId, string name)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return null;

            var lower = trimmed.ToLower();
            var list = await GetAllAsync(
                d => d.DepartmentID == departmentId
                     && ((d.DivisionShort != null && d.DivisionShort.ToLower() == lower)
                         || (d.DivisionLong != null && d.DivisionLong.ToLower() == lower)),
                null,
                null).ConfigureAwait(false);
            return list.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task EnsureDefaultDepartmentDivisionExistsAsync(Guid departmentId)
        {
            const string divisionName = "Department";
            var lower = divisionName.ToLowerInvariant();
            var existing = (await _repository.GetAllAsync(
                    d => d.DepartmentID == departmentId
                         && ((d.DivisionShort != null && d.DivisionShort.ToLower() == lower)
                             || (d.DivisionLong != null && d.DivisionLong.ToLower() == lower)),
                    null,
                    null).ConfigureAwait(false))
                .FirstOrDefault();
            if (existing != null)
                return;

            await UpsertAsync(
                new DepartmentDivisionsDto(
                    Guid.Empty,
                    departmentId,
                    divisionName,
                    divisionName,
                    true),
                isSuperUser: true).ConfigureAwait(false);
        }

        public override Task<PagedList<DepartmentDivisionsDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentDivisions, bool>>? filter = null,
            Expression<Func<DepartmentDivisions, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<DepartmentDivisions, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(DepartmentDivisions), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<DepartmentDivisions, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(DepartmentDivisions.DivisionShort),
                nameof(DepartmentDivisions.DivisionLong)
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

