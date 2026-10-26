using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for export log service operations.
    /// </summary>
    public interface IExportLogService : IGenericService<ExportLogDto, ExportLog>
    {
        /// <summary>
        /// Retrieves an export log record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The export log DTO if found; otherwise, null.</returns>
        Task<ExportLogDto?> GetByIdAsync(int id);
    }
}

