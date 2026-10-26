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
    /// Service for managing requester configuration operations using the generic service pattern.
    /// </summary>
    public class RequesterConfigurationService : GenericService<RequesterConfigurationDto, RequesterConfiguration>, IRequesterConfigurationService
    {
        private readonly IRequesterConfigurationRepository _requesterConfigurationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigurationService"/> class.
        /// </summary>
        /// <param name="repository">The requester configuration repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterConfigurationService(
            IRequesterConfigurationRepository repository,
            IMapper mapper,
            ILogger<RequesterConfigurationService> logger) : base(repository, mapper, "ReqConID")
        {
            _requesterConfigurationRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a requester configuration by its primary key (bigint/long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester configuration DTO if found; otherwise, null.</returns>
        public async Task<RequesterConfigurationDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _requesterConfigurationRepository.GetByIdAsync(id, "ReqConID");
            if (entity == null)
                return null;

            return _mapper.Map<RequesterConfigurationDto>(entity);
        }
    }
}

