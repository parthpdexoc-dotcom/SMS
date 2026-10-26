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
    /// Service for managing HR status operations using the generic service pattern.
    /// </summary>
    public class HRStatusService : GenericService<HRStatusDto, HRStatus>, IHRStatusService
    {
        private readonly IHRStatusRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HRStatusService"/> class.
        /// </summary>
        /// <param name="repository">The HR status repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public HRStatusService(
            IHRStatusRepository repository,
            IMapper mapper,
            ILogger<HRStatusService> logger) : base(repository, mapper, "HRStatusID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an HR status by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR status DTO if found; otherwise, null.</returns>
        public async Task<HRStatusDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "HRStatusID");
            if (entity == null)
                return null;

            return _mapper.Map<HRStatusDto>(entity);
        }
    }
}

