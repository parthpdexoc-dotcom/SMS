using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing emergency contact operations using the generic service pattern.
    /// </summary>
    public class EmergencyContactService : GenericService<EmergencyContactDto, EmergencyContact>, IEmergencyContactService
    {
        private readonly IEmergencyContactRepository _emergencyContactRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyContactService"/> class.
        /// </summary>
        /// <param name="repository">The emergency contact repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmergencyContactService(
            IEmergencyContactRepository repository,
            IMapper mapper,
            ILogger<EmergencyContactService> logger) : base(repository, mapper, "EmergencyContactID")
        {
            _emergencyContactRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an emergency contact by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The emergency contact DTO if found; otherwise, null.</returns>
        public async Task<EmergencyContactDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _emergencyContactRepository.GetByIdAsync(id, "EmergencyContactID");
            if (entity == null)
                return null;

            return _mapper.Map<EmergencyContactDto>(entity);
        }

        /// <summary>
        /// Creates or updates an emergency contact.
        /// </summary>
        public override async Task<EmergencyContactDto> UpsertAsync(EmergencyContactDto dto, bool isSuperUser = false)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return await base.UpsertAsync(dto, isSuperUser);
        }
    }
}

