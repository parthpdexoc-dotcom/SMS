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
    /// Service for managing employee contacts operations using the generic service pattern.
    /// </summary>
    public class EmployeeContactsService : GenericService<EmployeeContactsDto, EmployeeContacts>, IEmployeeContactsService
    {
        private readonly IEmployeeContactsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactsService"/> class.
        /// </summary>
        /// <param name="repository">The employee contacts repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="employeeService">The employee service for foreign key validation.</param>
        public EmployeeContactsService(
            IEmployeeContactsRepository repository,
            IMapper mapper,
            ILogger<EmployeeContactsService> logger,
            IEmployeeService employeeService) : base(repository, mapper, "ContactID")
        {
            _repository = repository;
            _mapper = mapper;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves an employee contact by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee contact DTO if found; otherwise, null.</returns>
        public async Task<EmployeeContactsDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ContactID");
            if (entity == null)
                return null;

            return _mapper.Map<EmployeeContactsDto>(entity);
        }

        /// <summary>
        /// Creates or updates an employee contact with foreign key validation.
        /// </summary>
        /// <param name="dto">The employee contact DTO to create or update.</param>
        /// <returns>The created or updated employee contact DTO.</returns>
        public override async Task<EmployeeContactsDto> UpsertAsync(EmployeeContactsDto dto, bool isSuperUser = false)
        {
            // Validate FkEmployeeID foreign key (nullable - only validate if provided)
            if (dto.FkEmployeeID.HasValue && dto.FkEmployeeID.Value != Guid.Empty)
            {
                var employee = await _employeeService.GetByIdAsync(dto.FkEmployeeID.Value);
                if (employee == null)
                {
                    throw new ArgumentException($"Referenced Employee with ID {dto.FkEmployeeID.Value} does not exist.");
                }
            }

            // No phone fields on EmployeeContactsDto; delegate to base implementation.
            return await base.UpsertAsync(dto, isSuperUser);
        }

        /// <summary>
        /// Creates or updates multiple employee contacts with foreign key validation.
        /// </summary>
        /// <param name="dtos">The collection of employee contact DTOs to create or update.</param>
        /// <returns>The collection of created or updated employee contact DTOs.</returns>
        public override async Task<IEnumerable<EmployeeContactsDto>> BulkUpsertAsync(IEnumerable<EmployeeContactsDto> dtos, bool isSuperUser = false)
        {
            var dtoList = dtos.ToList();

            // Get all unique foreign key values
            var employeeIds = dtoList
                .Where(d => d.FkEmployeeID.HasValue && d.FkEmployeeID.Value != Guid.Empty)
                .Select(d => d.FkEmployeeID.Value)
                .Distinct()
                .ToList();

            // Validate all Employee foreign keys exist
            if (employeeIds.Any())
            {
                var existingEmployees = await _employeeService.GetAllAsync(e => employeeIds.Contains(e.EmployeeID));
                var existingEmployeeIds = existingEmployees.Select(e => e.EmployeeID).ToHashSet();

                var missingEmployeeIds = employeeIds.Where(id => !existingEmployeeIds.Contains(id)).ToList();
                if (missingEmployeeIds.Any())
                {
                    throw new ArgumentException(
                        $"Referenced Employees with IDs {string.Join(", ", missingEmployeeIds)} do not exist.");
                }
            }

            // All foreign keys validated, proceed with normal bulk upsert
            return await base.BulkUpsertAsync(dtoList, isSuperUser);
        }
    }
}

