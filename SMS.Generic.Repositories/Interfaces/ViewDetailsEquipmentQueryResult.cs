using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>Single-request bundle for view-details equipment and rank context (loaded via joins/includes).</summary>
    public sealed class ViewDetailsEquipmentQueryResult
    {
        public List<RequestEquipment> RequestEquipment { get; init; } = new();
        public List<PostEquipment> PostEquipment { get; init; } = new();
        public List<PostPosition> RequestPositions { get; init; } = new();
        public List<EmployeeTenure> EmployeeTenures { get; init; } = new();

        /// <summary>
        /// Per post, department equipment IDs defined on <see cref="PostEquipment"/> (any effective dates).
        /// Request-level rows with the same ID are overridden when viewing that post.
        /// </summary>
        public Dictionary<Guid, HashSet<Guid>> PostDepartmentEquipmentIdsByPostId { get; init; } =
            new();
    }
}
