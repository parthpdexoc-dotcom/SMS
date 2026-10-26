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
    /// Service for managing eligible type operations using the generic service pattern.
    /// </summary>
    public class EligibleTypeService : GenericService<EligibleTypeDto, EligibleType>, IEligibleTypeService
    {
        private readonly IEligibleTypeRepository _eligibleTypeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EligibleTypeService"/> class.
        /// </summary>
        /// <param name="repository">The eligible type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EligibleTypeService(
            IEligibleTypeRepository repository,
            IMapper mapper,
            ILogger<EligibleTypeService> logger) : base(repository, mapper, "EligibleCID")
        {
            _eligibleTypeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an eligible type by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The eligible type DTO if found; otherwise, null.</returns>
        public async Task<EligibleTypeDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _eligibleTypeRepository.GetByIdAsync(id, "EligibleCID");
            if (entity == null)
                return null;

            return _mapper.Map<EligibleTypeDto>(entity);
        }
    }
}

