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
    /// Service for managing login/logout history operations using the generic service pattern.
    /// </summary>
    public class LogInOutHistoryService : GenericService<LogInOutHistoryDto, LogInOutHistory>, ILogInOutHistoryService
    {
        private readonly ILogInOutHistoryRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOutHistoryService"/> class.
        /// </summary>
        /// <param name="repository">The login/logout history repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public LogInOutHistoryService(
            ILogInOutHistoryRepository repository,
            IMapper mapper,
            ILogger<LogInOutHistoryService> logger) : base(repository, mapper, "LogInOutHistoryID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a login/logout history record by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The login/logout history DTO if found; otherwise, null.</returns>
        public async Task<LogInOutHistoryDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "LogInOutHistoryID");
            if (entity == null)
                return null;

            return _mapper.Map<LogInOutHistoryDto>(entity);
        }
    }
}

