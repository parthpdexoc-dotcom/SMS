using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for department fee repository operations, extending IGenericRepository.
    /// </summary>
    public interface IDepartmentFeeRepository : IGenericRepository<DepartmentFee>
    {
        // Add department fee-specific repository methods here if needed
        // Otherwise, leave empty - generic methods are inherited
    }
}
