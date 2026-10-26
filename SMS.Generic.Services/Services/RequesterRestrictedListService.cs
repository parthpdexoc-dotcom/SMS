using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing requester restricted list operations using the generic service pattern.
    /// </summary>
    public class RequesterRestrictedListService : GenericService<RequesterRestrictedListDto, RequesterRestrictedList>, IRequesterRestrictedListService
    {
        private readonly IRequesterRestrictedListRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IRequesterService _requesterService;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterRestrictedListService"/> class.
        /// </summary>
        /// <param name="repository">The requester restricted list repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterRestrictedListService(
            IRequesterRestrictedListRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequesterRestrictedListService> logger,
            IRequesterService requesterService,
            IEmployeeService employeeService) : base(repository, mapper, "RequesterRestrictID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _requesterService = requesterService;
            _employeeService = employeeService;
        }
        public override async Task<RequesterRestrictedListDto> UpsertAsync(RequesterRestrictedListDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // For updates, prevent editing expired records
            if (IsUpdateOperation(dto) && dto.RequesterRestrictID is Guid id && id != Guid.Empty)
            {
                var existingDto = await GetByIdAsync(id);

                if (existingDto != null && existingDto.EffectiveEndDate < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.CANNOT_EDIT_EXPIRED_REQUESTER_RESTRICTION));
                }
            }

            // Validate RequesterID exists if provided
            await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterID, "Requester");

            // Validate EmployeeID foreign key exists
            await ValidateForeignKeyAsync<IEmployeeService, EmployeeDto>(_employeeService, dto.EmployeeID, "Employee");

            // Validate for duplicate RequesterID + EmployeeID combination (scoped to current user)
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.RequesterID == dto.RequesterID && e.EmployeeID == dto.EmployeeID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUESTER_RESTRICTION);

            // Set DepartmentID from token to ensure users can only work with their own department's data
            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets all requester restricted list records with navigation properties included.
        /// </summary>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <param name="requesterId">When set, only rows for this requester are returned.</param>
        /// <returns>Paged requester restricted list response DTOs with names.</returns>
        public async Task<PagedList<RequesterRestrictedListResponseDto>> GetAllWithNamesPagedAsync(
            Expression<Func<RequesterRestrictedList, bool>>? filter = null,
            Expression<Func<RequesterRestrictedList, object>>[]? includes = null,
            PageParam? pageParam = null,
            Guid? requesterId = null)
        {
            // Always include Requester and Employee navigation properties for name mapping
            var includesList = new List<Expression<Func<RequesterRestrictedList, object>>>
            {
                x => x.Requester,
                x => x.Employee
            };

            if (includes != null && includes.Length > 0)
            {
                includesList.AddRange(includes);
            }

            var includesArray = includesList.ToArray();
            var effectivePageParam = pageParam ?? new PageParam();

            Expression<Func<RequesterRestrictedList, bool>>? combinedFilter = filter;
            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                Expression<Func<RequesterRestrictedList, bool>> byRequester = x => x.RequesterID == requesterId.Value;
                combinedFilter = combinedFilter == null
                    ? byRequester
                    : ExpressionCombiner.AndAlso(combinedFilter, byRequester);
            }

            var paged = await GetAllPagedAsync(combinedFilter, effectivePageParam, includesArray).ConfigureAwait(false);
            var response = _mapper.Map<IEnumerable<RequesterRestrictedListResponseDto>>(paged.Data);
            return new PagedList<RequesterRestrictedListResponseDto>(response, effectivePageParam, paged.TotalRecords);
        }

        /// <summary>
        /// Gets a requester restricted list record by ID with navigation properties included.
        /// </summary>
        /// <param name="id">The requester restricted list ID.</param>
        /// <returns>The requester restricted list response DTO with names, or null if not found.</returns>
        public async Task<RequesterRestrictedListResponseDto?> GetByIdWithNamesAsync(Guid id)
        {
            // Use GetAllAsync with filter since GetByIdAsync only supports one include
            var includes = new Expression<Func<RequesterRestrictedList, object>>[]
            {
                x => x.Requester,
                x => x.Employee
            };

            var entities = (await GetAllAsync(
                filter: e => e.RequesterRestrictID == id,
                includes: includes,
                pageParam: null)).ToList();

            var entity = entities.FirstOrDefault();
            if (entity == null)
            {
                return null;
            }

            return _mapper.Map<RequesterRestrictedListResponseDto>(entity);
        }
        public async Task<List<RequesterRestrictedListDto>> GetRequesterIdsByEmployeeId(Guid employeeId, DateOnly startDate, DateOnly endDate)
        {
            var requesters = await _repository.GetRequesterIdsByEmployeeId(employeeId,startDate, endDate);
            return _mapper.Map<List<RequesterRestrictedListDto>>(requesters);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetAvailableEmployeeOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null)
        {
            IReadOnlyList<Guid>? excludeEmployeeIds = null;
            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                excludeEmployeeIds = (await GetAllAsync(
                    filter: x => x.RequesterID == requesterId.Value,
                    includes: null,
                    pageParam: null).ConfigureAwait(false))
                    .Select(x => x.EmployeeID)
                    .Distinct()
                    .ToList();
            }

            return await _employeeService.GetOptionsAsync(searchTerm, excludeEmployeeIds).ConfigureAwait(false);
        }
    }
}

