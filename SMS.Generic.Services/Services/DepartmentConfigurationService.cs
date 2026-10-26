using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department configuration operations using the generic service pattern.
    /// </summary>
    public class DepartmentConfigurationService : GenericService<DepartmentConfigurationDto, DepartmentConfiguration>, IDepartmentConfigurationService
    {
        private readonly IDepartmentConfigurationRepository _departmentConfigurationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentConfigurationService"/> class.
        /// </summary>
        /// <param name="repository">The department configuration repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="departmentRepository">The department repository for foreign key validation.</param>
        /// <param name="tokenParser">The token parser instance (optional).</param>
        public DepartmentConfigurationService(
            IDepartmentConfigurationRepository repository,
            IMapper mapper,
            IDepartmentRepository departmentRepository,
            ITokenParser? tokenParser = null) : base(repository, mapper, "DeptConfigID", tokenParser)
        {
            _departmentConfigurationRepository = repository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Inserts a new department configuration or updates an existing one.
        /// Validates that DepartmentID is not duplicate when creating a new configuration.
        /// </summary>
        /// <param name="dto">The department configuration DTO to insert or update.</param>
        /// <returns>The upserted department configuration DTO.</returns>
        public override async Task<DepartmentConfigurationDto> UpsertAsync(DepartmentConfigurationDto dto, bool isSuperUser = false)
        {
            // Validate DepartmentID foreign key exists
            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentID, "DepartmentID").ConfigureAwait(false);
            if (department == null)
                throw new ArgumentException($"Department with ID {dto.DepartmentID} does not exist.");

            // Check if this is a create operation (DeptConfigID is null or empty)
            bool isCreate = dto.DeptConfigID == null || dto.DeptConfigID == Guid.Empty;

            if (isCreate)
            {
                // Validate that DepartmentID is not duplicate when creating
                var existingConfigs = await GetAllAsync(
                    filter: x => x.DepartmentID == dto.DepartmentID,
                    includes: null,
                    pageParam: null);

                if (existingConfigs.Any())
                {
                    throw new ArgumentException($"A configuration already exists for DepartmentID {dto.DepartmentID}. DepartmentID cannot be duplicate.");
                }
            }

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets a department configuration by DepartmentID.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>The department configuration if found; otherwise, null.</returns>
        public async Task<DepartmentConfigurationDto?> GetByDepartmentIdAsync(Guid departmentId)
        {
            var configs = await GetAllAsync(
                filter: x => x.DepartmentID == departmentId,
                includes: null,
                pageParam: null);

            return configs.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task EnsureExistsForDepartmentAsync(Guid departmentId)
        {
            var count = await _departmentConfigurationRepository
                .GetFilteredCountAsync(x => x.DepartmentID == departmentId).ConfigureAwait(false);

            if (count == 0)
            {
                var config = new DepartmentConfiguration
                {
                    DepartmentID = departmentId
                };
                await _departmentConfigurationRepository.UpsertAsync(config, "DeptConfigID").ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public Task<bool> DeleteByDepartmentIdAsync(Guid departmentId)
        {
            return DeleteByPropertyAsync("DepartmentID", departmentId);
        }
    }
}

