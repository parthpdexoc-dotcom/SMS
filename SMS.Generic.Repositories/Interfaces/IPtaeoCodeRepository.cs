using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for PTAEO_Code repository operations.
    /// </summary>
    public interface IPtaeoCodeRepository : IGenericRepository<PtaeoCode>
    {
        /// <summary>
        /// Deletes all records from the PtaeoCode table without affecting the PtaeoCodeID column definition.
        /// </summary>
        Task DeleteAllAsync(CancellationToken cancellationToken = default);
    }
}
