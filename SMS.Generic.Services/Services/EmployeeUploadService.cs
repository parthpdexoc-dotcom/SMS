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
    /// Service for managing employee upload operations using the generic service pattern.
    /// </summary>
    public class EmployeeUploadService : GenericService<EmployeeUploadDto, EmployeeUpload>, IEmployeeUploadService
    {
        private readonly IEmployeeUploadRepository _employeeUploadRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeUploadService"/> class.
        /// </summary>
        /// <param name="repository">The employee upload repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeUploadService(
            IEmployeeUploadRepository repository,
            IMapper mapper,
            ILogger<EmployeeUploadService> logger) : base(repository, mapper, "UploadID")
        {
            _employeeUploadRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an employee upload record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee upload DTO if found; otherwise, null.</returns>
        public async Task<EmployeeUploadDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _employeeUploadRepository.GetByIdAsync(id, "UploadID");
            if (entity == null)
                return null;

            return _mapper.Map<EmployeeUploadDto>(entity);
        }
    }
}

