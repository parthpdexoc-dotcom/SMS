using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department activity type operations using the generic service pattern.
    /// </summary>
    public class DepartmentActivityTypeService : GenericService<DepartmentActivityTypeDto, DepartmentActivityType>, IDepartmentActivityTypeService
    {
        private readonly IDepartmentActivityTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentActivityTypeService"/> class.
        /// </summary>
        /// <param name="repository">The department activity type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public DepartmentActivityTypeService(
            IDepartmentActivityTypeRepository repository,
            IMapper mapper,
            ILogger<DepartmentActivityTypeService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "DepartmentActivityTypeId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new DepartmentActivityTypeDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The DepartmentActivityTypeDto to insert or update.</param>
        /// <returns>The upserted DepartmentActivityTypeDto.</returns>
        public override async Task<DepartmentActivityTypeDto> UpsertAsync(DepartmentActivityTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate ActivityTypeShort within the same department
            if (!string.IsNullOrWhiteSpace(dto.ActivityTypeShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "ActivityTypeShort",
                    value: dto.ActivityTypeShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_ACTIVITY_TYPE_SHORT);
            }

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<DepartmentActivityTypeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentActivityType, bool>>? filter = null,
            Expression<Func<DepartmentActivityType, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<DepartmentActivityType, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(DepartmentActivityType), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<DepartmentActivityType, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                        nameof(DepartmentActivityType.ActivityTypeShort),
                        nameof(DepartmentActivityType.ActivityTypeLong)
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
