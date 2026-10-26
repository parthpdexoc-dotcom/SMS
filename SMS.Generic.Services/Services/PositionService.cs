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
    /// Service for managing position operations using the generic service pattern.
    /// </summary>
    public class PositionService : GenericService<PositionDto, Position>, IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionService"/> class.
        /// </summary>
        /// <param name="repository">The position repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PositionService(
            IPositionRepository repository,
            IMapper mapper,
            ILogger<PositionService> logger) : base(repository, mapper, "PositionID")
        {
            _positionRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a position by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The position DTO if found; otherwise, null.</returns>
        public async Task<PositionDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _positionRepository.GetByIdAsync(id, "PositionID");
            if (entity == null)
                return null;

            return _mapper.Map<PositionDto>(entity);
        }
    }
}

