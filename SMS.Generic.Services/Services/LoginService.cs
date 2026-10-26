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
    /// Service for managing login record operations using the generic service pattern.
    /// </summary>
    public class LoginService : GenericService<LoginRecordDto, Login>, ILoginService
    {
        private readonly ILoginRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="repository">The login repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public LoginService(
            ILoginRepository repository,
            IMapper mapper,
            ILogger<LoginService> logger) : base(repository, mapper, "LoID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a login record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The login record DTO if found; otherwise, null.</returns>
        public async Task<LoginRecordDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "LoID");
            if (entity == null)
                return null;

            return _mapper.Map<LoginRecordDto>(entity);
        }
    }
}

