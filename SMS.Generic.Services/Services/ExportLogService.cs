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
    /// Service for managing export log operations using the generic service pattern.
    /// </summary>
    public class ExportLogService : GenericService<ExportLogDto, ExportLog>, IExportLogService
    {
        private readonly IExportLogRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportLogService"/> class.
        /// </summary>
        /// <param name="repository">The export log repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ExportLogService(
            IExportLogRepository repository,
            IMapper mapper,
            ILogger<ExportLogService> logger) : base(repository, mapper, "ExportID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an export log record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value (ExportID).</param>
        /// <returns>The export log DTO if found; otherwise, null.</returns>
        public async Task<ExportLogDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_EXPORT_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ExportID");
            if (entity == null)
                return null;

            return _mapper.Map<ExportLogDto>(entity);
        }
    }
}

