using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for skill service operations.
    /// </summary>
    public interface ISkillService : IGenericService<SkillDto, Skill>
    {
        /// <summary>
        /// Validates that all skill IDs exist in the skill table using optimized batch operations.
        /// </summary>
        /// <param name="skillIds">The list of skill IDs to validate.</param>
        /// <exception cref="KeyNotFoundException">Thrown when one or more skill IDs are not found.</exception>
        Task ValidateSkillIdsAsync(List<Guid> skillIds);

        /// <summary>
        /// Ensures a default CPR skill exists for the department (case-insensitive short/long).
        /// </summary>
        Task EnsureCprSkillExistsForDepartmentAsync(Guid departmentId);

        /// <summary>
        /// Ensures a default Suspended skill exists for the department (case-insensitive short/long).
        /// </summary>
        Task EnsureSuspendedSkillExistsForDepartmentAsync(Guid departmentId);
    }
}
