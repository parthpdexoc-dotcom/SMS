using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for PTAEO_Code service operations.
    /// </summary>
    public interface IPtaeoCodeService : IGenericService<PtaeoCodeDto, PtaeoCode>
    {
        /// <summary>
        /// Deletes all records from the PtaeoCode table without affecting the PtaeoCodeID column definition.
        /// </summary>
        Task DeleteAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets lightweight PTAEO code options for dropdowns (composite label, searchable).
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? selectedId = null);
    }
}
