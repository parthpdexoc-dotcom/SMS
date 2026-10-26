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
    /// Service for managing HR employee type operations using the generic service pattern.
    /// </summary>
    public class HrEmployeeTypeService : GenericService<HrEmployeeTypeDto, HrEmployeeType>, IHrEmployeeTypeService
    {
        private readonly IHrEmployeeTypeRepository _hrEmployeeTypeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrEmployeeTypeService"/> class.
        /// </summary>
        /// <param name="repository">The HR employee type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public HrEmployeeTypeService(
            IHrEmployeeTypeRepository repository,
            IMapper mapper,
            ILogger<HrEmployeeTypeService> logger) : base(repository, mapper, "HrEmpTypeID")
        {
            _hrEmployeeTypeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an HR employee type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR employee type DTO if found; otherwise, null.</returns>
        public async Task<HrEmployeeTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _hrEmployeeTypeRepository.GetByIdAsync(id, "HrEmpTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<HrEmployeeTypeDto>(entity);
        }
    }
}

