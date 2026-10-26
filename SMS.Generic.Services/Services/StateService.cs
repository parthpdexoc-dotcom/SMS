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
    /// Service for managing state operations using the generic service pattern.
    /// </summary>
    public class StateService : GenericService<StateDto, State>, IStateService
    {
        private readonly IStateRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateService"/> class.
        /// </summary>
        /// <param name="repository">The state repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public StateService(
            IStateRepository repository,
            IMapper mapper,
            ILogger<StateService> logger) : base(repository, mapper, "StateID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a state by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The state DTO if found; otherwise, null.</returns>
        public async Task<StateDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "StateID");
            if (entity == null)
                return null;

            return _mapper.Map<StateDto>(entity);
        }
    }
}

