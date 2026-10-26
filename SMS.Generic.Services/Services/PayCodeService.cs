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
    /// Service for managing pay code operations using the generic service pattern.
    /// </summary>
    public class PayCodeService : GenericService<PayCodeDto, PayCode>, IPayCodeService
    {
        private readonly IPayCodeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayCodeService"/> class.
        /// </summary>
        /// <param name="repository">The pay code repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public PayCodeService(
            IPayCodeRepository repository,
            IMapper mapper,
            ILogger<PayCodeService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "PayCodeID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new PayCodeDto or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The PayCodeDto to insert or update.</param>
        /// <returns>The upserted PayCodeDto.</returns>
        public override async Task<PayCodeDto> UpsertAsync(PayCodeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (!string.IsNullOrWhiteSpace(dto.PayCodeValue))
            {
                await ValidateDuplicateAsync(
                    propertyName: "PayCodeValue",
                    value: dto.PayCodeValue,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_PAY_CODE_VALUE);
            }

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<PayCodeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<PayCode, bool>>? filter = null,
            Expression<Func<PayCode, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<PayCode, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(PayCode), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<PayCode, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(PayCode.PayCodeValue),
                nameof(PayCode.PayCodeDescription)
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

