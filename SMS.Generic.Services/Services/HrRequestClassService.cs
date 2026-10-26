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
    /// Service for managing HR request class operations using the generic service pattern.
    /// </summary>
    public class HrRequestClassService : GenericService<HrRequestClassDto, HrRequestClass>, IHrRequestClassService
    {
        private readonly IHrRequestClassRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrRequestClassService"/> class.
        /// </summary>
        /// <param name="repository">The HR request class repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public HrRequestClassService(
            IHrRequestClassRepository repository,
            IMapper mapper,
            ILogger<HrRequestClassService> logger) : base(repository, mapper, "HrRequestClassID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an HR request class by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR request class DTO if found; otherwise, null.</returns>
        public async Task<HrRequestClassDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "HrRequestClassID");
            if (entity == null)
                return null;

            return _mapper.Map<HrRequestClassDto>(entity);
        }
    }
}

