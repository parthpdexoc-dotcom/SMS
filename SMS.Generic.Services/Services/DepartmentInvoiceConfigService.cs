using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department invoice config operations using the generic service pattern.
    /// </summary>
    public class DepartmentInvoiceConfigService : GenericService<DepartmentInvoiceConfigDto, DepartmentInvoiceConfig>, IDepartmentInvoiceConfigService
    {
        private readonly IDepartmentInvoiceConfigRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentInvoiceConfigService"/> class.
        /// </summary>
        /// <param name="repository">The department invoice config repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public DepartmentInvoiceConfigService(
            IDepartmentInvoiceConfigRepository repository,
            IMapper mapper,
            ILogger<DepartmentInvoiceConfigService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "InvoiceConfigID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Gets department invoice configs filtered by the logged-in user's department for UI binding.
        /// </summary>
        public override async Task<IEnumerable<DepartmentInvoiceConfigDto>> GetAllAsync(
            Expression<Func<DepartmentInvoiceConfig, bool>>? filter = null,
            Expression<Func<DepartmentInvoiceConfig, object>>[] includes = null,
            PageParam pageParam = null)
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (departmentId.HasValue)
                filter = d => d.DepartmentID == departmentId.Value;
            return await base.GetAllAsync(filter, includes, pageParam);
        }

        /// <summary>
        /// Returns the configured fiscal year for a specific department.
        /// If there is no config or no FiscalYear (or it's invalid), it falls back to current year.
        /// </summary>
        public async Task<int> GetFiscalYearForDepartmentAsync(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return DateTime.UtcNow.Year;

            // Use repository directly to avoid token-based department scoping in GenericService.
            var config = (await _repository
                    .GetAllAsync(c => c.DepartmentID == departmentId, null, null)
                    .ConfigureAwait(false))
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(config?.FiscalYear) &&
                int.TryParse(config.FiscalYear, out var fiscalYear))
            {
                return fiscalYear;
            }

            return DateTime.UtcNow.Year;
        }
    }
}
