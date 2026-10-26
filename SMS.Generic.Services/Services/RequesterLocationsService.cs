using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing requester locations operations using the generic service pattern.
    /// </summary>
    public class RequesterLocationsService : GenericService<RequesterLocationsDto, RequesterLocations>, IRequesterLocationsService
    {
        private readonly IRequesterLocationsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IRequesterService _requesterService;
        private readonly IDepartmentResponseAreaService _responseAreaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterLocationsService"/> class.
        /// </summary>
        /// <param name="repository">The requester locations repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="departmentService">The department service for foreign key validation.</param>
        /// <param name="requesterService">The requester service for foreign key validation.</param>
        /// <param name="responseAreaService">The response area service for foreign key validation.</param>
        public RequesterLocationsService(
            IRequesterLocationsRepository repository,
            IMapper mapper,
            ILogger<RequesterLocationsService> logger,
            IDepartmentService departmentService,
            IRequesterService requesterService,
            IDepartmentResponseAreaService responseAreaService) : base(repository, mapper, "PkLocationID")
        {
            _repository = repository;
            _mapper = mapper;
            _departmentService = departmentService;
            _requesterService = requesterService;
            _responseAreaService = responseAreaService;
        }

        /// <summary>
        /// Retrieves a requester location by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester location DTO if found; otherwise, null.</returns>
        public async Task<RequesterLocationsDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PkLocationID");
            if (entity == null)
                return null;

            return _mapper.Map<RequesterLocationsDto>(entity);
        }

        /// <summary>
        /// Creates or updates a requester location with foreign key validation.
        /// </summary>
        /// <param name="dto">The requester location DTO to create or update.</param>
        /// <returns>The created or updated requester location DTO.</returns>
        public override async Task<RequesterLocationsDto> UpsertAsync(RequesterLocationsDto dto, bool isSuperUser = false)
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

            // Validate FKRequesterID foreign key (nullable - only validate if provided)
            if (dto.FKRequesterID.HasValue && dto.FKRequesterID.Value != Guid.Empty)
            {
                var requester = await _requesterService.GetByIdAsync(dto.FKRequesterID.Value);
                if (requester == null)
                {
                    throw new ArgumentException($"Referenced Requester with ID {dto.FKRequesterID.Value} does not exist.");
                }
            }

            // Validate FkDepartmentResponseAreaID foreign key (nullable - only validate if provided)
            if (dto.FkDepartmentResponseAreaID.HasValue && dto.FkDepartmentResponseAreaID.Value != Guid.Empty)
            {
                var responseArea = await _responseAreaService.GetByIdAsync(dto.FkDepartmentResponseAreaID.Value);
                if (responseArea == null)
                {
                    throw new ArgumentException($"Referenced DepartmentResponseArea with ID {dto.FkDepartmentResponseAreaID.Value} does not exist.");
                }
            }

            // All foreign keys validated, proceed with normal upsert
            return await base.UpsertAsync(dto, isSuperUser);
        }

        /// <summary>
        /// Creates or updates multiple requester locations with foreign key validation.
        /// </summary>
        /// <param name="dtos">The collection of requester location DTOs to create or update.</param>
        /// <returns>The collection of created or updated requester location DTOs.</returns>
        public override async Task<IEnumerable<RequesterLocationsDto>> BulkUpsertAsync(IEnumerable<RequesterLocationsDto> dtos, bool isSuperUser = false)
        {
            var dtoList = dtos.ToList();

            // Get all unique foreign key values
            var departmentIds = dtoList
                .Where(d => d.FkDepartmentID.HasValue && d.FkDepartmentID.Value != Guid.Empty)
                .Select(d => d.FkDepartmentID.Value)
                .Distinct()
                .ToList();

            var requesterIds = dtoList
                .Where(d => d.FKRequesterID.HasValue && d.FKRequesterID.Value != Guid.Empty)
                .Select(d => d.FKRequesterID.Value)
                .Distinct()
                .ToList();

            var responseAreaIds = dtoList
                .Where(d => d.FkDepartmentResponseAreaID.HasValue && d.FkDepartmentResponseAreaID.Value != Guid.Empty)
                .Select(d => d.FkDepartmentResponseAreaID.Value)
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

            // Validate all Requester foreign keys exist
            if (requesterIds.Any())
            {
                var existingRequesters = await _requesterService.GetAllAsync(r => requesterIds.Contains(r.RequesterID));
                var existingRequesterIds = existingRequesters.Select(r => r.RequesterID).ToHashSet();

                var missingRequesterIds = requesterIds.Where(id => !existingRequesterIds.Contains(id)).ToList();
                if (missingRequesterIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced Requesters with IDs {string.Join(", ", missingRequesterIds)} do not exist.");
                }
            }

            // Validate all DepartmentResponseArea foreign keys exist
            if (responseAreaIds.Any())
            {
                var existingDepartmentResponseAreas = await _responseAreaService.GetAllAsync(ra => responseAreaIds.Contains(ra.DepartmentResponseAreaID));
                var existingDepartmentResponseAreaIds = existingDepartmentResponseAreas.Select(ra => ra.DepartmentResponseAreaID).ToHashSet();

                var missingDepartmentResponseAreaIds = responseAreaIds.Where(id => !existingDepartmentResponseAreaIds.Contains(id)).ToList();
                if (missingDepartmentResponseAreaIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced DepartmentResponseAreas with IDs {string.Join(", ", missingDepartmentResponseAreaIds)} do not exist.");
                }
            }

            // All foreign keys validated, proceed with normal bulk upsert
            return await base.BulkUpsertAsync(dtoList, isSuperUser);
        }
    }
}

