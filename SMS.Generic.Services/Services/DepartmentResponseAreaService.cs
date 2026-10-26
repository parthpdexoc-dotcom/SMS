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
    /// Service for managing response area operations using the generic service pattern.
    /// </summary>
    public class DepartmentResponseAreaService : GenericService<DepartmentResponseAreaDto, DepartmentResponseArea>, IDepartmentResponseAreaService
    {
        private readonly IDepartmentResponseAreaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentResponseAreaService"/> class.
        /// </summary>
        /// <param name="repository">The response area repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public DepartmentResponseAreaService(
            IDepartmentResponseAreaRepository repository,
            IMapper mapper,
            ILogger<DepartmentResponseAreaService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "DepartmentResponseAreaID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new DepartmentResponseAreaDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The DepartmentResponseAreaDto to insert or update.</param>
        /// <returns>The upserted DepartmentResponseAreaDto.</returns>
        public override async Task<DepartmentResponseAreaDto> UpsertAsync(DepartmentResponseAreaDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate DepartmentResponseAreaShort within the same department
            if (!string.IsNullOrWhiteSpace(dto.DepartmentResponseAreaShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "DepartmentResponseAreaShort",
                    value: dto.DepartmentResponseAreaShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_RESPONSE_AREA_SHORT);
            }

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets response areas by department ID from the token.
        /// </summary>
        /// <returns>A collection of response areas for the department from the token.</returns>
        public async Task<IEnumerable<DepartmentResponseAreaDto>> GetAllActiveDepartmentAsync(Guid? selectedId = null)
        {
            var areas = await GetAllAsync(
                filter: x => x.DepartmentResponseAreaStatus == true ||
                             (selectedId.HasValue && x.DepartmentResponseAreaID == selectedId.Value),
                includes: null,
                pageParam: null).ConfigureAwait(false);

            // Ensure dropdown is sorted alphabetically by short name (A-Z)
            return areas.OrderBy(x => x.DepartmentResponseAreaShort);
        }

        public override Task<PagedList<DepartmentResponseAreaDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentResponseArea, bool>>? filter = null,
            Expression<Func<DepartmentResponseArea, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<DepartmentResponseArea, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(DepartmentResponseArea), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<DepartmentResponseArea, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(DepartmentResponseArea.DepartmentResponseAreaShort),
                nameof(DepartmentResponseArea.DepartmentResponseAreaLong)
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
