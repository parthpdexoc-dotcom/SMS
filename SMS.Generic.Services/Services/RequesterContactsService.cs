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
    /// Service for managing requester contacts operations using the generic service pattern.
    /// </summary>
    public class RequesterContactsService : GenericService<RequesterContactsDto, RequesterContacts>, IRequesterContactsService
    {
        private readonly IRequesterContactsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IRequesterService _requesterService;
        private readonly IRequesterContactTypesService _requesterContactTypesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactsService"/> class.
        /// </summary>
        /// <param name="repository">The requester contacts repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="departmentService">The department service for foreign key validation.</param>
        /// <param name="requesterService">The requester service for foreign key validation.</param>
        /// <param name="requesterContactTypesService">The requester contact types service for foreign key validation.</param>
        public RequesterContactsService(
            IRequesterContactsRepository repository,
            IMapper mapper,
            ILogger<RequesterContactsService> logger,
            IDepartmentService departmentService,
            IRequesterService requesterService,
            IRequesterContactTypesService requesterContactTypesService) : base(repository, mapper, "PkContactID")
        {
            _repository = repository;
            _mapper = mapper;
            _departmentService = departmentService;
            _requesterService = requesterService;
            _requesterContactTypesService = requesterContactTypesService;
        }

        /// <summary>
        /// Retrieves a requester contact by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester contact DTO if found; otherwise, null.</returns>
        public async Task<RequesterContactsDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PkContactID");
            if (entity == null)
                return null;

            return _mapper.Map<RequesterContactsDto>(entity);
        }

        /// <summary>
        /// Creates or updates a requester contact with foreign key validation.
        /// </summary>
        /// <param name="dto">The requester contact DTO to create or update.</param>
        /// <returns>The created or updated requester contact DTO.</returns>
        public override async Task<RequesterContactsDto> UpsertAsync(RequesterContactsDto dto, bool isSuperUser = false)
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

            // Validate FkRequesterID foreign key (nullable - only validate if provided)
            if (dto.FkRequesterID.HasValue && dto.FkRequesterID.Value != Guid.Empty)
            {
                var requester = await _requesterService.GetByIdAsync(dto.FkRequesterID.Value);
                if (requester == null)
                {
                    throw new ArgumentException($"Referenced Requester with ID {dto.FkRequesterID.Value} does not exist.");
                }
            }

            // Validate FkContactTypeID foreign key (nullable - only validate if provided)
            if (dto.FkContactTypeID.HasValue && dto.FkContactTypeID.Value > 0)
            {
                var contactType = await _requesterContactTypesService.GetByIdAsync(dto.FkContactTypeID.Value);
                if (contactType == null)
                {
                    throw new ArgumentException($"Referenced RequesterContactTypes with ID {dto.FkContactTypeID.Value} does not exist.");
                }
            }

            // All foreign keys validated, proceed with normal upsert
            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Creates or updates multiple requester contacts with foreign key validation.
        /// </summary>
        /// <param name="dtos">The collection of requester contact DTOs to create or update.</param>
        /// <returns>The collection of created or updated requester contact DTOs.</returns>
        public override async Task<IEnumerable<RequesterContactsDto>> BulkUpsertAsync(IEnumerable<RequesterContactsDto> dtos, bool isSuperUser = false)
        {
            var dtoList = dtos.ToList();

            // Get all unique foreign key values
            var departmentIds = dtoList
                .Where(d => d.FkDepartmentID.HasValue && d.FkDepartmentID.Value != Guid.Empty)
                .Select(d => d.FkDepartmentID.Value)
                .Distinct()
                .ToList();

            var requesterIds = dtoList
                .Where(d => d.FkRequesterID.HasValue && d.FkRequesterID.Value != Guid.Empty)
                .Select(d => d.FkRequesterID.Value)
                .Distinct()
                .ToList();

            var contactTypeIds = dtoList
                .Where(d => d.FkContactTypeID.HasValue && d.FkContactTypeID.Value > 0)
                .Select(d => d.FkContactTypeID.Value)
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

            // Validate all RequesterContactTypes foreign keys exist
            if (contactTypeIds.Any())
            {
                var existingContactTypes = await _requesterContactTypesService.GetAllAsync(ct => contactTypeIds.Contains(ct.PKRequesterContactTypeID));
                var existingContactTypeIds = existingContactTypes.Select(ct => ct.PKRequesterContactTypeID).ToHashSet();

                var missingContactTypeIds = contactTypeIds.Where(id => !existingContactTypeIds.Contains(id)).ToList();
                if (missingContactTypeIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced RequesterContactTypes with IDs {string.Join(", ", missingContactTypeIds)} do not exist.");
                }
            }

            // All foreign keys validated, proceed with normal bulk upsert
            return await base.BulkUpsertAsync(dtoList, isSuperUser);
        }
    }
}

