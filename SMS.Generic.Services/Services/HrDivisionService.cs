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
    /// Service for managing HR division operations using the generic service pattern.
    /// </summary>
    public class HrDivisionService : GenericService<HrDivisionDto, HrDivision>, IHrDivisionService
    {
        private readonly IHrDivisionRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrDivisionService"/> class.
        /// </summary>
        /// <param name="repository">The HR division repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public HrDivisionService(
            IHrDivisionRepository repository,
            IMapper mapper,
            ILogger<HrDivisionService> logger) : base(repository, mapper, "HrDivisionID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an HR division by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR division DTO if found; otherwise, null.</returns>
        public async Task<HrDivisionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "HrDivisionID");
            if (entity == null)
                return null;

            return _mapper.Map<HrDivisionDto>(entity);
        }
    }
}

