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
    /// Service for managing requester contact types operations using the generic service pattern.
    /// </summary>
    public class RequesterContactTypesService : GenericService<RequesterContactTypesDto, RequesterContactTypes>, IRequesterContactTypesService
    {
        private readonly IRequesterContactTypesRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactTypesService"/> class.
        /// </summary>
        /// <param name="repository">The requester contact types repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterContactTypesService(
            IRequesterContactTypesRepository repository,
            IMapper mapper,
            ILogger<RequesterContactTypesService> logger) : base(repository, mapper, "PKRequesterContactTypeID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a requester contact type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester contact type DTO if found; otherwise, null.</returns>
        public async Task<RequesterContactTypesDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PKRequesterContactTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<RequesterContactTypesDto>(entity);
        }
    }
}

