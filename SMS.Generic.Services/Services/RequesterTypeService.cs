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
    /// Service for managing requester type operations using the generic service pattern.
    /// </summary>
    public class RequesterTypeService : GenericService<RequesterTypeDto, RequesterType>, IRequesterTypeService
    {
        private readonly IRequesterTypeRepository _requesterTypeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterTypeService"/> class.
        /// </summary>
        /// <param name="repository">The requester type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterTypeService(
            IRequesterTypeRepository repository,
            IMapper mapper,
            ILogger<RequesterTypeService> logger) : base(repository, mapper, "PkRequesterTypeID")
        {
            _requesterTypeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a requester type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester type DTO if found; otherwise, null.</returns>
        public async Task<RequesterTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _requesterTypeRepository.GetByIdAsync(id, "PkRequesterTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<RequesterTypeDto>(entity);
        }
    }
}

