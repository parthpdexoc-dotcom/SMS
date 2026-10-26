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
    /// Service for managing close request details operations using the generic service pattern.
    /// </summary>
    public class CloseRequestDetailsService : GenericService<CloseRequestDetailsDto, CloseRequestDetails>, ICloseRequestDetailsService
    {
        private readonly ICloseRequestDetailsRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseRequestDetailsService"/> class.
        /// </summary>
        /// <param name="repository">The close request details repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public CloseRequestDetailsService(
            ICloseRequestDetailsRepository repository,
            IMapper mapper,
            ILogger<CloseRequestDetailsService> logger) : base(repository, mapper, "CloseRequestDetailID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a close request detail by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The close request detail DTO if found; otherwise, null.</returns>
        public async Task<CloseRequestDetailsDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "CloseRequestDetailID");
            if (entity == null)
                return null;

            return _mapper.Map<CloseRequestDetailsDto>(entity);
        }
    }
}

