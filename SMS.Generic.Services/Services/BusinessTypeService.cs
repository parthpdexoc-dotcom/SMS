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
    /// Service for managing business type operations using the generic service pattern.
    /// </summary>
    public class BusinessTypeService : GenericService<BusinessTypeDto, BusinessType>, IBusinessTypeService
    {
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTypeService"/> class.
        /// </summary>
        /// <param name="repository">The business type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="employeeService">The employee service (kept for backward compatibility but no longer used for audit tracking).</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public BusinessTypeService(
            IBusinessTypeRepository repository,
            IMapper mapper,
            ILogger<BusinessTypeService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "BusinessTypeID", tokenParser)
        {
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new BusinessTypeDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The BusinessTypeDto to insert or update.</param>
        /// <returns>The upserted BusinessTypeDto.</returns>
        public override async Task<BusinessTypeDto> UpsertAsync(BusinessTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate BusinessTypeShort within the same department
            await ValidateDuplicateAsync(
                propertyName: "BusinessTypeShort",
                value: dto.BusinessTypeShort,
                dto: dto,
                isDepartmentScoped: true,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_BUSINESS_TYPE_SHORT);

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<BusinessTypeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<BusinessType, bool>>? filter = null,
            Expression<Func<BusinessType, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<BusinessType, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(BusinessType), "x");
                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<BusinessType, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(BusinessType.BusinessTypeShort),
                nameof(BusinessType.BusinessTypeLong)
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

