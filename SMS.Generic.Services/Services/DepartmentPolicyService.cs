using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Repositories.Repositories;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department policy operations using the generic service pattern.
    /// </summary>
    public class DepartmentPolicyService : GenericService<DepartmentPolicyDto, DepartmentPolicy>, IDepartmentPolicyService
    {

        private readonly IDepartmentPolicyRepository _departmentPolicyRepository;
        private readonly ILogger<DepartmentPolicyService> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentPolicyService"/> class.
        /// </summary>
        /// <param name="repository">The department policy repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public DepartmentPolicyService(
            IDepartmentPolicyRepository repository,
            IMapper mapper,
            ILogger<DepartmentPolicyService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "DepartmentPolicyID", tokenParser)
        {
            _departmentPolicyRepository = repository;
            _logger = logger;
        }

        public async Task<DepartmentPolicyDto?> GetDepartmentPolicyConfigurationByDepartmentId(Guid departmentId)
        {
            try
            {
                if (departmentId == Guid.Empty)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_CANNOT_BE_EMPTY), nameof(departmentId));

                var result = await GetAllAsync(
                     filter: d => d.DepartmentID == departmentId,
                     includes: new Expression<Func<DepartmentPolicy, object>>[]
                     {
                    d => d.DepartmentConfiguration
                     },
                     pageParam: null
                 );
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get department policy configuration for DepartmentId {DepartmentId}", departmentId);
                return null;
            }
        }

        public async Task EnsureExistsForDepartmentAsync(Guid departmentId)
        {
            var count = await _departmentPolicyRepository
                .GetFilteredCountAsync(x => x.DepartmentID == departmentId).ConfigureAwait(false);

            if (count == 0)
            {
                var config = new DepartmentPolicy
                {
                    DepartmentID = departmentId
                };
                await _departmentPolicyRepository.UpsertAsync(config, "DepartmentPolicyID").ConfigureAwait(false);
            }
        }
    }
}
