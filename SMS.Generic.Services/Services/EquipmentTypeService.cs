using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Models.Constants;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing equipment type operations using the generic service pattern.
    /// </summary>
    public class EquipmentTypeService : GenericService<EquipmentTypeDto, EquipmentType>, IEquipmentTypeService
    {
        private readonly IEquipmentTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentTypeService"/> class.
        /// </summary>
        /// <param name="repository">The equipment type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EquipmentTypeService(
            IEquipmentTypeRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<EquipmentTypeService> logger) : base(repository, mapper, "EquipementTypeID", tokenParser)
        {
            _repository = repository;
            _tokenParser = tokenParser;
            _mapper = mapper;
        }
        /// <summary>
        /// Inserts a new equipment type or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The equipment type DTO to insert or update.</param>
        /// <returns>The upserted equipment type DTO.</returns>
        public override async Task<EquipmentTypeDto> UpsertAsync(EquipmentTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate EquipementTypeShort within the same department (same name allowed for different DepartmentID)
            if (!string.IsNullOrWhiteSpace(dto.EquipementTypeShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "EquipementTypeShort",
                    value: dto.EquipementTypeShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_EQUIPMENT_TYPE_SHORT);
            }

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<EquipmentTypeDto>> SearchAsync(
             string? searchTerm,
             PageParam? pageParam = null,
             Expression<Func<EquipmentType, bool>>? filter = null,
             Expression<Func<EquipmentType, object>>[]? includes = null,
             params string[] searchableProperties)
        {
            Expression<Func<EquipmentType, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(EquipmentType), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<EquipmentType, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(EquipmentType.EquipementTypeShort),
                nameof(EquipmentType.EquipementTypeLong)
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


