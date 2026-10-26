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
    /// Service for managing request activity type operations using the generic service pattern.
    /// </summary>
    public class RequestActivityTypeService : GenericService<RequestActivityTypeDto, RequestActivityType>, IRequestActivityTypeService
    {
        private readonly IRequestActivityTypeRepository _requestActivityTypeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestActivityTypeService"/> class.
        /// </summary>
        /// <param name="repository">The request activity type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestActivityTypeService(
            IRequestActivityTypeRepository repository,
            IMapper mapper,
            ILogger<RequestActivityTypeService> logger) : base(repository, mapper, "PkRequestActivityTypeID")
        {
            _requestActivityTypeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a request activity type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The request activity type DTO if found; otherwise, null.</returns>
        public async Task<RequestActivityTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _requestActivityTypeRepository.GetByIdAsync(id, "PkRequestActivityTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<RequestActivityTypeDto>(entity);
        }
    }
}

