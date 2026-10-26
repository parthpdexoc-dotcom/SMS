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
    /// Service for managing division admin on duty operations using the generic service pattern.
    /// </summary>
    public class DivisionAdminOnDutyService : GenericService<DivisionAdminOnDutyDto, DivisionAdminOnDuty>, IDivisionAdminOnDutyService
    {
        private readonly IDivisionAdminOnDutyRepository _divisionAdminOnDutyRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionAdminOnDutyService"/> class.
        /// </summary>
        /// <param name="repository">The division admin on duty repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public DivisionAdminOnDutyService(
            IDivisionAdminOnDutyRepository repository,
            IMapper mapper,
            ILogger<DivisionAdminOnDutyService> logger) : base(repository, mapper, "DivisionAdminOnDutyID")
        {
            _divisionAdminOnDutyRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a division admin on duty by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The division admin on duty DTO if found; otherwise, null.</returns>
        public async Task<DivisionAdminOnDutyDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _divisionAdminOnDutyRepository.GetByIdAsync(id, "DivisionAdminOnDutyID");
            if (entity == null)
                return null;

            return _mapper.Map<DivisionAdminOnDutyDto>(entity);
        }
    }
}

