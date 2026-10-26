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
    /// Service for managing email process operations using the generic service pattern.
    /// </summary>
    public class EmailProcessService : GenericService<EmailProcessDto, EmailProcess>, IEmailProcessService
    {
        private readonly IEmailProcessRepository _emailProcessRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailProcessService"/> class.
        /// </summary>
        /// <param name="repository">The email process repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailProcessService(
            IEmailProcessRepository repository,
            IMapper mapper,
            ILogger<EmailProcessService> logger) : base(repository, mapper, "EmailProcessID")
        {
            _emailProcessRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an email process by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email process DTO if found; otherwise, null.</returns>
        public async Task<EmailProcessDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _emailProcessRepository.GetByIdAsync(id, "EmailProcessID");
            if (entity == null)
                return null;

            return _mapper.Map<EmailProcessDto>(entity);
        }
    }
}

