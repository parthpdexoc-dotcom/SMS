using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing employee admin operations using the generic service pattern.
    /// </summary>
    public class EmployeeAdminService : GenericService<EmployeeAdminDto, EmployeeAdmin>, IEmployeeAdminService
    {
        private readonly IEmployeeAdminRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly IAdminTypeService _adminTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeAdminService"/> class.
        /// </summary>
        /// <param name="repository">The employee admin repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeAdminService(
            IEmployeeAdminRepository repository,
            IMapper mapper,
            ILogger<EmployeeAdminService> logger,
            IEmployeeService employeeService,
            IAdminTypeService adminTypeService
            ) : base(repository, mapper, "EmployeeAdminID")
        {
            _repository = repository;
            _mapper = mapper;
            _employeeService = employeeService;
            _adminTypeService = adminTypeService;
        }

        /// <summary>
        /// Inserts a new employee admin or updates an existing one.
        /// </summary>
        /// <param name="dto">The employee admin DTO to insert or update.</param>
        /// <returns>The upserted employee admin DTO.</returns>
        public override async Task<EmployeeAdminDto> UpsertAsync(EmployeeAdminDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            await ValidateForeignKeyAsync<IEmployeeService, EmployeeDto>(_employeeService, dto.EmployeeID, "Employee");

            await ValidateForeignKeyAsync<IAdminTypeService, AdminTypeDto>(_adminTypeService, dto.AdminTypeID, "AdminType");

            // Validate for duplicate EmployeeID + AdminTypeID combination (globally unique)
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.EmployeeID == dto.EmployeeID && e.AdminTypeID == dto.AdminTypeID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_EMPLOYEE_ADMIN_ASSIGNMENT);

            return await base.UpsertAsync(dto);
        }
    }
}
