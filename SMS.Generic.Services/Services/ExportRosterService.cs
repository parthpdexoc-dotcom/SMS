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
    /// Service for managing export roster operations using the generic service pattern.
    /// </summary>
    public class ExportRosterService : GenericService<ExportRosterDto, ExportRoster>, IExportRosterService
    {
        private readonly IExportRosterRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRosterService"/> class.
        /// </summary>
        /// <param name="repository">The export roster repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ExportRosterService(
            IExportRosterRepository repository,
            IMapper mapper,
            ILogger<ExportRosterService> logger) : base(repository, mapper, "SL")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an export roster record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The export roster DTO if found; otherwise, null.</returns>
        public async Task<ExportRosterDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "SL");
            if (entity == null)
                return null;

            return _mapper.Map<ExportRosterDto>(entity);
        }
    }
}

