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
    /// Service for managing email communication operations using the generic service pattern.
    /// </summary>
    public class EmailCommunicationService : GenericService<EmailCommunicationDto, EmailCommunication>, IEmailCommunicationService
    {
        private readonly IEmailCommunicationRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailCommunicationService"/> class.
        /// </summary>
        /// <param name="repository">The email communication repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailCommunicationService(
            IEmailCommunicationRepository repository,
            IMapper mapper,
            ILogger<EmailCommunicationService> logger) : base(repository, mapper, "ID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an email communication by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email communication DTO if found; otherwise, null.</returns>
        public async Task<EmailCommunicationDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ID");
            if (entity == null)
                return null;

            return _mapper.Map<EmailCommunicationDto>(entity);
        }
    }
}

