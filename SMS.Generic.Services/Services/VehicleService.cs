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
    /// Service for managing vehicle operations using the generic service pattern.
    /// </summary>
    public class VehicleService : GenericService<VehicleDto, Vehicle>, IVehicleService
    {
        private readonly IVehicleRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleService"/> class.
        /// </summary>
        /// <param name="repository">The vehicle repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public VehicleService(
            IVehicleRepository repository,
            IMapper mapper,
            ILogger<VehicleService> logger) : base(repository, mapper, "VehicleID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a vehicle by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The vehicle DTO if found; otherwise, null.</returns>
        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "VehicleID");
            if (entity == null)
                return null;

            return _mapper.Map<VehicleDto>(entity);
        }
    }
}

