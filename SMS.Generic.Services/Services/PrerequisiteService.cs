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
    /// Service for managing prerequisite operations using the generic service pattern.
    /// </summary>
    public class PrerequisiteService : GenericService<PrerequisiteDto, Prerequisite>, IPrerequisiteService
    {
        private readonly IPrerequisiteRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IPrerequisiteTypeService _prerequisiteTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteService"/> class.
        /// </summary>
        /// <param name="repository">The prerequisite repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="departmentService">The department service for foreign key validation.</param>
        /// <param name="prerequisiteTypeService">The prerequisite type service for foreign key validation.</param>
        public PrerequisiteService(
            IPrerequisiteRepository repository,
            IMapper mapper,
            ILogger<PrerequisiteService> logger,
            IDepartmentService departmentService,
            IPrerequisiteTypeService prerequisiteTypeService) : base(repository, mapper, "PkPrerequisiteID")
        {
            _repository = repository;
            _mapper = mapper;
            _departmentService = departmentService;
            _prerequisiteTypeService = prerequisiteTypeService;
        }

        /// <summary>
        /// Retrieves a prerequisite by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The prerequisite DTO if found; otherwise, null.</returns>
        public async Task<PrerequisiteDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PkPrerequisiteID");
            if (entity == null)
                return null;

            return _mapper.Map<PrerequisiteDto>(entity);
        }

        /// <summary>
        /// Inserts or updates a prerequisite with foreign key validation.
        /// </summary>
        /// <param name="dto">The prerequisite DTO to insert or update.</param>
        /// <returns>The inserted or updated prerequisite DTO.</returns>
        public override async Task<PrerequisiteDto> UpsertAsync(PrerequisiteDto dto, bool isSuperUser = false)
        {
            // Validate FkDepartmentID foreign key (nullable - only validate if provided)
            if (dto.FkDepartmentID.HasValue && dto.FkDepartmentID.Value != Guid.Empty)
            {
                var department = await _departmentService.GetByIdAsync(dto.FkDepartmentID.Value);
                if (department == null)
                {
                    throw new ArgumentException($"Referenced Department with ID {dto.FkDepartmentID.Value} does not exist.");
                }
            }

            // Validate FkPrerequisiteTypeID foreign key (nullable - only validate if provided)
            if (dto.FkPrerequisiteTypeID.HasValue && dto.FkPrerequisiteTypeID.Value > 0)
            {
                var prerequisiteType = await _prerequisiteTypeService.GetByIdAsync(dto.FkPrerequisiteTypeID.Value);
                if (prerequisiteType == null)
                {
                    throw new ArgumentException($"Referenced PrerequisiteType with ID {dto.FkPrerequisiteTypeID.Value} does not exist.");
                }
            }

            // All foreign keys validated, proceed with normal upsert
            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Performs bulk insert or update operations with foreign key validation.
        /// </summary>
        /// <param name="dtos">The collection of prerequisite DTOs to insert or update.</param>
        /// <returns>The collection of inserted or updated prerequisite DTOs.</returns>
        public override async Task<IEnumerable<PrerequisiteDto>> BulkUpsertAsync(IEnumerable<PrerequisiteDto> dtos, bool isSuperUser = false)
        {
            var dtoList = dtos.ToList();

            // Get all unique foreign key values
            var departmentIds = dtoList
                .Where(d => d.FkDepartmentID.HasValue && d.FkDepartmentID.Value != Guid.Empty)
                .Select(d => d.FkDepartmentID.Value)
                .Distinct()
                .ToList();

            var prerequisiteTypeIds = dtoList
                .Where(d => d.FkPrerequisiteTypeID.HasValue && d.FkPrerequisiteTypeID.Value > 0)
                .Select(d => d.FkPrerequisiteTypeID.Value)
                .Distinct()
                .ToList();

            // Validate all Department foreign keys exist
            if (departmentIds.Any())
            {
                var existingDepartments = await _departmentService.GetAllAsync(d => departmentIds.Contains(d.DepartmentID));
                var existingDepartmentIds = existingDepartments.Select(d => d.DepartmentID).ToHashSet();

                var missingDepartmentIds = departmentIds.Where(id => !existingDepartmentIds.Contains(id)).ToList();
                if (missingDepartmentIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced Departments with IDs {string.Join(", ", missingDepartmentIds)} do not exist.");
                }
            }

            // Validate all PrerequisiteType foreign keys exist
            if (prerequisiteTypeIds.Any())
            {
                var existingPrerequisiteTypes = await _prerequisiteTypeService.GetAllAsync(pt => prerequisiteTypeIds.Contains(pt.PkPrerequisiteTypeID));
                var existingPrerequisiteTypeIds = existingPrerequisiteTypes.Select(pt => pt.PkPrerequisiteTypeID).ToHashSet();

                var missingPrerequisiteTypeIds = prerequisiteTypeIds.Where(id => !existingPrerequisiteTypeIds.Contains(id)).ToList();
                if (missingPrerequisiteTypeIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced PrerequisiteTypes with IDs {string.Join(", ", missingPrerequisiteTypeIds)} do not exist.");
                }
            }

            // All foreign keys validated, proceed with normal bulk upsert
            return await base.BulkUpsertAsync(dtoList, isSuperUser);
        }
    }
}

