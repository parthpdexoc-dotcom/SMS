using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for block date service operations.
    /// </summary>
    public interface IBlockDateService : IGenericService<BlockDateDto, BlockDate>
    {
        // Add block date-specific service methods here if needed
        // Otherwise, leave empty - generic methods are inherited
    }
}
