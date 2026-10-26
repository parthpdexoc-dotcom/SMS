using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing fit standards operations using the generic service pattern.
    /// </summary>
    public class FitStandardsService : GenericService<FitStandardsDto, FitStandards>, IFitStandardsService
    {
        private readonly IFitStandardsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FitStandardsService"/> class.
        /// </summary>
        /// <param name="repository">The fit standards repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="departmentService">The department service for foreign key validation.</param>
        public FitStandardsService(
            IFitStandardsRepository repository,
            IMapper mapper,
            ILogger<FitStandardsService> logger,
            IDepartmentService departmentService) : base(repository, mapper, "FitStandardID")
        {
            _repository = repository;
            _mapper = mapper;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrieves a fit standards record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The fit standards DTO if found; otherwise, null.</returns>
        public async Task<FitStandardsDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_FIT_STANDARD_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "FitStandardID");
            if (entity == null)
                return null;

            return _mapper.Map<FitStandardsDto>(entity);
        }

        /// <summary>
        /// Creates or updates a fit standards record with foreign key validation.
        /// </summary>
        /// <param name="dto">The fit standards DTO to create or update.</param>
        /// <returns>The created or updated fit standards DTO.</returns>
        public override async Task<FitStandardsDto> UpsertAsync(FitStandardsDto dto, bool isSuperUser = false)
        {
            // Validate DepartmentID foreign key if provided
            if (dto.DepartmentID.HasValue && dto.DepartmentID.Value != Guid.Empty)
            {
                var department = await _departmentService.GetByIdAsync(dto.DepartmentID.Value);
                if (department == null)
                {
                    throw new ArgumentException($"Referenced department with ID {dto.DepartmentID.Value} does not exist.");
                }
            }

            // Proceed with normal upsert
            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Creates or updates multiple fit standards records with foreign key validation.
        /// </summary>
        /// <param name="dtos">The fit standards DTOs to create or update.</param>
        /// <returns>The created or updated fit standards DTOs.</returns>
        public override async Task<IEnumerable<FitStandardsDto>> BulkUpsertAsync(IEnumerable<FitStandardsDto> dtos, bool isSuperUser = false)
        {
            var dtoList = dtos.ToList();

            // Get all unique department IDs
            var departmentIds = dtoList
                .Where(d => d.DepartmentID.HasValue && d.DepartmentID.Value != Guid.Empty)
                .Select(d => d.DepartmentID.Value)
                .Distinct()
                .ToList();

            // Validate all department IDs exist
            if (departmentIds.Any())
            {
                var existingDepartments = await _departmentService.GetAllAsync(d => departmentIds.Contains(d.DepartmentID));
                var existingDepartmentIds = existingDepartments.Select(d => d.DepartmentID).ToHashSet();

                var missingIds = departmentIds.Where(id => !existingDepartmentIds.Contains(id)).ToList();
                if (missingIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced departments with IDs {string.Join(", ", missingIds)} do not exist.");
                }
            }

            // Proceed with normal bulk upsert
            return await base.BulkUpsertAsync(dtoList, isSuperUser);
        }
    }
}

