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
    /// Service for managing estimate operations using the generic service pattern.
    /// </summary>
    public class EstimateService : GenericService<EstimateDto, Estimate>, IEstimateService
    {
        private readonly IEstimateRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EstimateService"/> class.
        /// </summary>
        /// <param name="repository">The estimate repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EstimateService(
            IEstimateRepository repository,
            IMapper mapper,
            ILogger<EstimateService> logger) : base(repository, mapper, "EstimateID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an estimate by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The estimate DTO if found; otherwise, null.</returns>
        public async Task<EstimateDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "EstimateID");
            if (entity == null)
                return null;

            return _mapper.Map<EstimateDto>(entity);
        }
    }
}

