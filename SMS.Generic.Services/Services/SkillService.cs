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
    /// Service for managing skill operations using the generic service pattern.
    /// </summary>
    public class SkillService : GenericService<SkillDto, Skill>, ISkillService
    {
        private readonly ISkillRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillService"/> class.
        /// </summary>
        /// <param name="repository">The skill repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public SkillService(
            ISkillRepository repository,
            IMapper mapper,
            ILogger<SkillService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "SkillID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new SkillDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The SkillDto to insert or update.</param>
        /// <returns>The upserted SkillDto.</returns>
        public override async Task<SkillDto> UpsertAsync(SkillDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate SkillShort within the same department (same name allowed for different DepartmentID)
            if (!string.IsNullOrWhiteSpace(dto.SkillShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "SkillShort",
                    value: dto.SkillShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_SKILL_SHORT);
            }

            return await base.UpsertAsync(dto);
        }
        public async Task ValidateSkillIdsAsync(List<Guid> skillIds)
        {
            if (skillIds == null || !skillIds.Any())
            {
                return; // Nothing to validate
            }

            // Remove duplicates to avoid unnecessary database calls
            var distinctSkillIds = skillIds.Distinct().ToList();

            // Get all existing skills in a single batch query
            var existingSkills = await GetAllAsync(s => distinctSkillIds.Contains(s.SkillID));

            // Find missing skill IDs using set operations (O(n) complexity)
            var existingSkillIds = existingSkills.Select(s => s.SkillID).ToHashSet();
            var missingSkillIds = distinctSkillIds.Where(id => !existingSkillIds.Contains(id)).ToList();

            if (missingSkillIds.Any())
            {
                throw new KeyNotFoundException($"The following skill IDs were not found: {string.Join(", ", missingSkillIds)}");
            }
        }

        /// <summary>
        /// Searches skills by short/long name with optional active filter.
        /// </summary>
        public override Task<PagedList<SkillDto>> SearchAsync(
        string? searchTerm,
        PageParam? pageParam = null,
        Expression<Func<Skill, bool>>? filter = null,
        Expression<Func<Skill, object>>[]? includes = null,
        params string[] searchableProperties)
        {
            Expression<Func<Skill, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(Skill), "x");
                var combinedBody = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));
                baseFilter = Expression.Lambda<Func<Skill, bool>>(combinedBody, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties == null || searchableProperties.Length == 0)
                    ? new[] { nameof(Skill.SkillShort), nameof(Skill.SkillLong) }
                    : searchableProperties;

            return base.SearchAsync(
                searchTerm,
                pageParam,
                baseFilter,
                includes,
                effectiveSearchableProperties);
        }

        /// <inheritdoc />
        public async Task EnsureCprSkillExistsForDepartmentAsync(Guid departmentId)
        {
            var existing = (await _repository.GetAllAsync(
                    x => x.DepartmentID == departmentId &&
                         ((x.SkillShort != null && x.SkillShort.ToLower() == "cpr") ||
                          (x.SkillLong != null && x.SkillLong.ToLower() == "cpr")),
                    null,
                    null).ConfigureAwait(false))
                .FirstOrDefault();
            if (existing != null)
                return;

            await _repository.UpsertAsync(
                new Skill
                {
                    SkillID = Guid.Empty,
                    DepartmentID = departmentId,
                    SkillShort = "CPR",
                    SkillLong = "CPR",
                    SkillStatus = true
                },
                "SkillID").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EnsureSuspendedSkillExistsForDepartmentAsync(Guid departmentId)
        {
            var existing = (await _repository.GetAllAsync(
                    x => x.DepartmentID == departmentId &&
                         ((x.SkillShort != null && x.SkillShort.ToLower() == "suspended") ||
                          (x.SkillLong != null && x.SkillLong.ToLower() == "suspended")),
                    null,
                    null).ConfigureAwait(false))
                .FirstOrDefault();
            if (existing != null)
                return;

            await _repository.UpsertAsync(
                new Skill
                {
                    SkillID = Guid.Empty,
                    DepartmentID = departmentId,
                    SkillShort = "Suspended",
                    SkillLong = "Suspended",
                    SkillStatus = true
                },
                "SkillID").ConfigureAwait(false);
        }
    }
}

