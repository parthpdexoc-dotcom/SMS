using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for EmployeeTenure operations.
    /// Promotions must be handled ONLY by INSERT. Existing records must never be updated by API input.
    /// </summary>
    public class EmployeeTenureService : GenericService<EmployeeTenureDto, EmployeeTenure>, IEmployeeTenureService
    {
        private readonly IEmployeeTenureRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;

        /// <summary>
        /// Initializes a new instance of the EmployeeTenureService.
        /// </summary>
        /// <param name="repository">The employee tenure repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        /// <param name="departmentRepository">Repository for validating department exists (avoids circular dependency with IDepartmentService).</param>
        public EmployeeTenureService(
            IEmployeeTenureRepository repository,
            IMapper mapper,
            ILogger<EmployeeTenureService> logger,
            ITokenParser tokenParser,
            IDepartmentRepository departmentRepository)
            : base(repository, mapper, "EmployeeTenureID", tokenParser)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Creates a new EmployeeTenure record. Updates are not allowed - only INSERT operations are permitted.
        /// </summary>
        /// <param name="dto">The EmployeeTenure DTO containing the data to insert.</param>
        /// <returns>The created EmployeeTenure DTO.</returns>
        public override async Task<EmployeeTenureDto> UpsertAsync(EmployeeTenureDto dto, bool isSuperUser = false)
        {
            try
            {
                var preparedDto = await PrepareTenureForInsertAsync(dto);

                // Set existing IsCurrent = true records to false before creating new record
                await _repository.UpdateAsync(
                    e => e.EmployeeID == preparedDto.EmployeeID.Value && e.IsCurrent,
                    e => e.IsCurrent = false);

                // Insert the new record
                return await base.UpsertAsync(preparedDto,isSuperUser);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override async Task<IEnumerable<EmployeeTenureDto>> BulkUpsertAsync(IEnumerable<EmployeeTenureDto> dtos, bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtosList = dtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DTOS_COLLECTION_EMPTY), nameof(dtos));

            if (dtosList
                .Where(x => x.EmployeeID.HasValue)
                .GroupBy(x => x.EmployeeID!.Value)
                .Any(g => g.Count() > 1))
            {
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    "Bulk insert supports only one EmployeeTenure record per employee at a time.");
            }

            var preparedDtos = new List<EmployeeTenureDto>();
            foreach (var dto in dtosList)
            {
                preparedDtos.Add(await PrepareTenureForInsertAsync(dto));
            }

            var employeeIds = preparedDtos
                .Where(x => x.EmployeeID.HasValue)
                .Select(x => x.EmployeeID!.Value)
                .Distinct()
                .ToList();

            if (employeeIds.Any())
            {
                await _repository.UpdateAsync(
                    e => employeeIds.Contains(e.EmployeeID) && e.IsCurrent,
                    e => e.IsCurrent = false);
            }

            return await base.BulkUpsertAsync(preparedDtos, isSuperUser);
        }

        private async Task<EmployeeTenureDto> PrepareTenureForInsertAsync(EmployeeTenureDto dto)
        {
            ValidateDtoParameter(dto);

            bool isNewRecord = !dto.EmployeeTenureID.HasValue || dto.EmployeeTenureID.Value == Guid.Empty;
            if (!isNewRecord)
            {
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    "Updates are not allowed. Promotions must be handled ONLY by INSERT.");
            }

            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentID.Value, "DepartmentID").ConfigureAwait(false);
            if (department == null)
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), "Department with ID " + dto.DepartmentID.Value + " not found.");

            var existingTenures = (await GetAllAsync(
                e => e.EmployeeID == dto.EmployeeID.Value,
                null,
                null)).ToList();

            if (dto.EmployeeTypeID.HasValue && existingTenures.Any(e => e.EmployeeTypeID == dto.EmployeeTypeID.Value))
            {
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                     ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_EMPLOYEE_TENURE));
            }

            dto.DateOfHire = dto.DateOfHire;
            // Using GetCurrentDepartmentDateByTimeZoneId(department.TimeZoneId)
            // because the user's token is not available during Department Admin creation,
            // so the timezone cannot be retrieved from the token at that stage.
            dto.DateOfPromotion = dto.DateOfPromotion ?? GetCurrentDepartmentDateByTimeZoneId(department.TimeZoneId);

            if (existingTenures.Count > 0)
            {
                var firstHireDate = existingTenures
                    .Where(e => e.DateOfHire.HasValue)
                    .Min(e => e.DateOfHire);

                if (dto.DateOfHire.HasValue && firstHireDate.HasValue &&
                    dto.DateOfHire.Value != firstHireDate.Value)
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DATE_OF_HIRE_CANNOT_BE_CHANGED));
                }

                dto.DateOfHire = firstHireDate ?? dto.DateOfHire ?? GetCurrentDepartmentDateByTimeZoneId(department.TimeZoneId);

                var lastPromotionDate = existingTenures
                    .Where(e => e.DateOfPromotion.HasValue)
                    .Max(e => e.DateOfPromotion);

                if (lastPromotionDate.HasValue && dto.DateOfPromotion.Value <= lastPromotionDate.Value)
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PROMOTION_DATE_MUST_BE_GREATER));
                }
            }
            else
            {
                dto.DateOfHire = dto.DateOfHire ?? GetCurrentDepartmentDateByTimeZoneId(department.TimeZoneId);
            }

            dto.IsCurrent = true;
            dto.EmployeeTenureID = null;
            return dto;
        }

        /// <inheritdoc />
        public async Task<PagedList<EmployeeTenureDto>> GetByEmployeeIdAsync(Guid employeeId, PageParam? pagination = null)
        {
            var page = pagination == null ? new PageParam() : new PageParam(pagination);
            if (string.IsNullOrWhiteSpace(page.SortBy))
            {
                page.SortBy = nameof(EmployeeTenure.DateOfPromotion);
                page.SortDirection = "asc";
            }

            var pagedItems = await GetAllAsync(
                e => e.EmployeeID == employeeId,
                new Expression<Func<EmployeeTenure, object>>[] { e => e.EmployeeType },
                page).ConfigureAwait(false);
            var totalRecords = await GetFilteredCountAsync(e => e.EmployeeID == employeeId).ConfigureAwait(false);

            return new PagedList<EmployeeTenureDto>(pagedItems, page, totalRecords);
        }
    }
}


