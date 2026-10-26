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
    /// Service for managing employee contact type operations using the generic service pattern.
    /// </summary>
    public class EmployeeContactTypeService : GenericService<EmployeeContactTypeDto, EmployeeContactType>, IEmployeeContactTypeService
    {
        private readonly IEmployeeContactTypeRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactTypeService"/> class.
        /// </summary>
        /// <param name="repository">The employee contact type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeContactTypeService(
            IEmployeeContactTypeRepository repository,
            IMapper mapper,
            ILogger<EmployeeContactTypeService> logger) : base(repository, mapper, "PkEmployeeContactTypeID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an employee contact type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee contact type DTO if found; otherwise, null.</returns>
        public async Task<EmployeeContactTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PkEmployeeContactTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<EmployeeContactTypeDto>(entity);
        }
    }
}

