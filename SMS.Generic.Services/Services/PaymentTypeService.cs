using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing payment type operations using the generic service pattern.
    /// </summary>
    public class PaymentTypeService : GenericService<PaymentTypeDto, PaymentType>, IPaymentTypeService
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentTypeService"/> class.
        /// </summary>
        public PaymentTypeService(
            IPaymentTypeRepository repository,
            IMapper mapper,
            ILogger<PaymentTypeService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "PaymentTypeID", tokenParser)
        {
            _repository = repository;
            _tokenParser = tokenParser;
        }

        public override async Task<PaymentTypeDto> UpsertAsync(PaymentTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (dto.DepartmentID == Guid.Empty)
            {
                var departmentId = _tokenParser.GetDepartmentID();
                if (!departmentId.HasValue)
                    throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

                dto.DepartmentID = departmentId.Value;
            }

            await ValidateDuplicateAsync(
                propertyName: nameof(PaymentType.Name),
                value: dto.Name,
                dto: dto,
                isDepartmentScoped: true,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_PAYMENT_TYPE_NAME);
            if (dto.IsDefault)
            {
                var existingDepartmentPaymentTypes = (await GetAllAsync(x => x.DepartmentID == dto.DepartmentID && x.IsDefault,null,null).ConfigureAwait(false)).ToList();
                if (dto.IsDefaultAlreadyExist(existingDepartmentPaymentTypes, out var existingDefaultName))
                    throw new WebApiException(HttpStatusCode.BadRequest,((int)HttpStatusCode.BadRequest).ToString(),ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_PAYMENT_TYPE_DEFAULT,existingDefaultName));
            }

            return await base.UpsertAsync(dto);
        }

        /// <inheritdoc />
        public async Task<string?> GetNameByIdAsync(Guid paymentTypeId, CancellationToken cancellationToken = default)
        {
            if (paymentTypeId == Guid.Empty)
                return null;

            var paymentType = await GetByIdAsync(paymentTypeId).ConfigureAwait(false);
            return string.IsNullOrWhiteSpace(paymentType?.Name) ? null : paymentType.Name.Trim();
        }

        /// <inheritdoc />
        public async Task EnsureDefaultPaymentTypesExistAsync(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return;

            foreach (PaymentTypeSeed seed in Enum.GetValues<PaymentTypeSeed>())
            {
                var name = seed.ToString();
                var lower = name.ToLowerInvariant();
                var existing = (await _repository.GetAllAsync(
                        x => x.DepartmentID == departmentId
                             && x.Name != null
                             && x.Name.ToLower() == lower,
                        null,
                        null).ConfigureAwait(false))
                    .FirstOrDefault();
                if (existing != null)
                    continue;

                await _repository.UpsertAsync(
                    new PaymentType
                    {
                        DepartmentID = departmentId,
                        Name = name,
                        IsDefault = seed == PaymentTypeSeed.Cash
                    },
                    "PaymentTypeID").ConfigureAwait(false);
            }
        }

        public override Task<PagedList<PaymentTypeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<PaymentType, bool>>? filter = null,
            Expression<Func<PaymentType, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<PaymentType, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(PaymentType), "x");
                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));
                baseFilter = Expression.Lambda<Func<PaymentType, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[] { nameof(PaymentType.Name) };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                baseFilter,
                includes,
                effectiveSearchableProperties);
        }
    }
}
