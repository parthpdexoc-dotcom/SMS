using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Models.Constants;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing requester operations using the generic service pattern.
    /// </summary>
    public class RequesterService : GenericService<RequesterDto, Requester>, IRequesterService
    {
        private readonly IRequesterRepository _repository;
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IBusinessTypeService _businessTypeService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterService"/> class.
        /// </summary>
        /// <param name="repository">The requester repository.</param>
        /// <param name="requestRepository">The request repository.</param>
        /// <param name="postRepository">The post repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="serviceProvider">Used to resolve request/manager services without circular constructor DI.</param>
        public RequesterService(
            IRequesterRepository repository,
            IRequestRepository requestRepository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequesterService> logger,
            IBusinessTypeService businessTypeService,
            IServiceProvider serviceProvider) : base(repository, mapper, "RequesterID", tokenParser)
        {
            _repository = repository;
            _requestRepository = requestRepository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _businessTypeService = businessTypeService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Inserts a new requester or updates an existing one.
        /// </summary>
        /// <param name="dto">The requester DTO to insert or update.</param>
        /// <returns>The upserted requester DTO.</returns>
        public override async Task<RequesterDto> UpsertAsync(RequesterDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate ParentID if IsChild is true
            if (dto.IsChild == true && (!dto.ParentID.HasValue || dto.ParentID.Value == Guid.Empty))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUESTER_PARENT_ID_REQUIRED_WHEN_CHILD));

            // Validate ParentID exists if provided (when IsChild is true)
            if (dto.IsChild == true && dto.ParentID.HasValue)
                await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(this, dto.ParentID.Value, "Requester");

            // Validate BusinessTypeID exists if provided
            await ValidateForeignKeyAsync<IBusinessTypeService, BusinessTypeDto>(_businessTypeService, dto.BusinessTypeID, "BusinessType");

            if (dto.IsLiquorLicenseRequired == true && string.IsNullOrWhiteSpace(dto.LiquorLicense))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUESTER_LIQUOR_LICENSE_REQUIRED));

            return await base.UpsertAsync(dto);
        }

        /// <inheritdoc />
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is Guid requesterId)
            {
                var hasRequests = await _requestRepository.GetFilteredCountAsync(r => r.RequesterID == requesterId) > 0;
                if (hasRequests)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUESTER_HAS_REQUESTS_CANNOT_DELETE));
                }
            }

            return await base.DeleteAsync(id);
        }
        public async Task<PagedList<RequesterResponseDto>> GetAllRequesterWithDetailsAsync(PageParam pageParam)
        {
            Expression<Func<Requester, object>>[] includes =
            {
                e => e.BusinessType,
                e => e.RequesterManagers
            };
            var effectivePageParam = pageParam ?? new PageParam();

            // Optional filter: when current user has Requester Manager admin type, restrict to requesters
            // where the logged-in employee is configured as a requester manager.
            Expression<Func<Requester, bool>>? filter = null;

            var adminScope = AdminRoleScopeHelper.GetScope(_tokenParser);
            if (adminScope.IsRequesterManagerOnly && adminScope.EmployeeId.HasValue)
            {
                var employeeGuid = adminScope.EmployeeId.Value;
                filter = r => r.RequesterManagers.Any(rm => rm.EmployeeID == employeeGuid);
            }

            var paged = await GetAllPagedAsync(filter, effectivePageParam, includes).ConfigureAwait(false);
            var dtos = _mapper.Map<List<RequesterResponseDto>>(paged.Data);

            if (adminScope.IsRequesterManagerOnly && adminScope.EmployeeId.HasValue)
            {
                var employeeGuid = adminScope.EmployeeId.Value;
                dtos = dtos
                    .Select(dto =>
                    {
                        dto.RequesterManagers = (dto.RequesterManagers ?? new List<RequesterManagerDto>())
                            .Where(rm => rm.EmployeeID == employeeGuid || rm.IsEmployeeShared)
                            .ToList();
                        return dto;
                    })
                    .ToList();
            }

            return new PagedList<RequesterResponseDto>(dtos, paged, paged.TotalRecords);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
            RequesterRequestSummaryFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Guid>? requesterManagerIds = null;

            if (!_tokenParser.IsDepartmentAdmin())
            {
                var employeeId = _tokenParser.GetEmployeeID();
                if (!employeeId.HasValue)
                    return Array.Empty<RequesterRequestSummaryDto>();

                requesterManagerIds = await _serviceProvider.GetRequiredService<IRequesterManagerService>()
                    .GetRequesterManagerIdsByEmployeeIdAsync(employeeId.Value)
                    .ConfigureAwait(false);

                if (requesterManagerIds.Count == 0)
                    return Array.Empty<RequesterRequestSummaryDto>();
            }

            return await _serviceProvider.GetRequiredService<IRequestService>()
                .GetRequesterRequestSummariesAsync(filter, requesterManagerIds, cancellationToken)
                .ConfigureAwait(false);
        }

        public override Task<PagedList<RequesterDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<Requester, bool>>? filter = null,
            Expression<Func<Requester, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<Requester, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(Requester), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<Requester, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(Requester.Name),
                nameof(Requester.DepartmentDefinedNumber),
                nameof(Requester.Email),
                nameof(Requester.Phone),
                nameof(Requester.PrimaryContact),
                nameof(Requester.City),
                nameof(Requester.State)
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

