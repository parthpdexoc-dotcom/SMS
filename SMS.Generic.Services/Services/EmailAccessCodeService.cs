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
    /// Service for managing email access code operations using the generic service pattern.
    /// </summary>
    public class EmailAccessCodeService : GenericService<EmailAccessCodeDto, EmailAccessCode>, IEmailAccessCodeService
    {
        private readonly IEmailAccessCodeRepository _emailAccessCodeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAccessCodeService"/> class.
        /// </summary>
        /// <param name="repository">The email access code repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailAccessCodeService(
            IEmailAccessCodeRepository repository,
            IMapper mapper,
            ILogger<EmailAccessCodeService> logger) : base(repository, mapper, "SL")
        {
            _emailAccessCodeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an email access code by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email access code DTO if found; otherwise, null.</returns>
        public async Task<EmailAccessCodeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _emailAccessCodeRepository.GetByIdAsync(id, "SL");
            if (entity == null)
                return null;

            return _mapper.Map<EmailAccessCodeDto>(entity);
        }
    }
}

