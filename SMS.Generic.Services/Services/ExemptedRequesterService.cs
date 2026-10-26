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
    /// Service for managing exempted requester operations using the generic service pattern.
    /// </summary>
    public class ExemptedRequesterService : GenericService<ExemptedRequesterDto, ExemptedRequester>, IExemptedRequesterService
    {
        private readonly IExemptedRequesterRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemptedRequesterService"/> class.
        /// </summary>
        /// <param name="repository">The exempted requester repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ExemptedRequesterService(
            IExemptedRequesterRepository repository,
            IMapper mapper,
            ILogger<ExemptedRequesterService> logger) : base(repository, mapper, "ExemptedRequestersID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an exempted requester record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The exempted requester DTO if found; otherwise, null.</returns>
        public async Task<ExemptedRequesterDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ExemptedRequestersID");
            if (entity == null)
                return null;

            return _mapper.Map<ExemptedRequesterDto>(entity);
        }
    }
}

