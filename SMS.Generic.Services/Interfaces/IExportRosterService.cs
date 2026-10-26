using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for export roster service operations.
    /// </summary>
    public interface IExportRosterService : IGenericService<ExportRosterDto, ExportRoster>
    {
        /// <summary>
        /// Retrieves an export roster record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The export roster DTO if found; otherwise, null.</returns>
        Task<ExportRosterDto?> GetByIdAsync(int id);
    }
}

