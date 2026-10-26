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
    /// Service for managing prerequisite type operations using the generic service pattern.
    /// </summary>
    public class PrerequisiteTypeService : GenericService<PrerequisiteTypeDto, PrerequisiteType>, IPrerequisiteTypeService
    {
        private readonly IPrerequisiteTypeRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteTypeService"/> class.
        /// </summary>
        /// <param name="repository">The prerequisite type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PrerequisiteTypeService(
            IPrerequisiteTypeRepository repository,
            IMapper mapper,
            ILogger<PrerequisiteTypeService> logger) : base(repository, mapper, "PkPrerequisiteTypeID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a prerequisite type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The prerequisite type DTO if found; otherwise, null.</returns>
        public async Task<PrerequisiteTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PkPrerequisiteTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<PrerequisiteTypeDto>(entity);
        }
    }
}

