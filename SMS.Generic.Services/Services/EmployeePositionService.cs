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
    /// Service for managing employee position operations using the generic service pattern.
    /// </summary>
    public class EmployeePositionService : GenericService<EmployeePositionDto, EmployeePosition>, IEmployeePositionService
    {
        private readonly IEmployeePositionRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePositionService"/> class.
        /// </summary>
        /// <param name="repository">The employee position repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeePositionService(
            IEmployeePositionRepository repository,
            IMapper mapper,
            ILogger<EmployeePositionService> logger) : base(repository, mapper, "EmployeePositionID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an employee position by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee position DTO if found; otherwise, null.</returns>
        public async Task<EmployeePositionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "EmployeePositionID");
            if (entity == null)
                return null;

            return _mapper.Map<EmployeePositionDto>(entity);
        }
    }
}

