using AutoMapper;
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
    /// Service for managing admin type operations using the generic service pattern.
    /// </summary>
    public class AdminTypeService : GenericService<AdminTypeDto, AdminType>, IAdminTypeService
    {
        private readonly IAdminTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTypeService"/> class.
        /// </summary>
        /// <param name="repository">The admin type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public AdminTypeService(
            IAdminTypeRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<AdminTypeService> logger) : base(repository, mapper, "AdminTypeID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new AdminTypeDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The AdminTypeDto to insert or update.</param>
        /// <returns>The upserted AdminTypeDto.</returns>
        public override async Task<AdminTypeDto> UpsertAsync(AdminTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // When ParentID points to an existing AdminType, mirror parent's IsSuperUser into IsSuperUserType.
            if (dto.ParentID is { } parentId && parentId != Guid.Empty &&
                await GetByIdAsync(parentId) is { } parent)
                dto.IsSuperUserType = parent.IsSuperUser;

            // Validate for duplicate AdminTypeShort (globally unique)
            await ValidateDuplicateAsync(
                propertyName: "AdminTypeShort",
                value: dto.AdminTypeShort,
                dto: dto,
                isDepartmentScoped: false,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_ADMIN_TYPE_SHORT);

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<AdminTypeDto>> SearchAsync(
    string? searchTerm,
    PageParam? pageParam = null,
    Expression<Func<AdminType, bool>>? filter = null,
    Expression<Func<AdminType, object>>[]? includes = null,
    params string[] searchableProperties)
        {
            filter ??= x => x.Status;

            var effectiveSearchableProperties =
                searchableProperties?.Length > 0
                    ? searchableProperties
                    : new[]
                    {
                nameof(AdminType.AdminTypeShort),
                nameof(AdminType.AdminTypeLong)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                filter,
                includes,
                effectiveSearchableProperties
            );
        }
    }
}

