using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee type service operations.
    /// </summary>
    public interface IEmployeeTypeService : IGenericService<EmployeeTypeDto, EmployeeType>
    {
        /// <summary>
        /// Updates the rank of an employee type with validation.
        /// </summary>
        /// <param name="employeeTypeId">The employee type ID.</param>
        /// <param name="rank">The new rank value.</param>
        Task UpdateRankAsync(Guid employeeTypeId, int rank);

        /// <summary>
        /// Same as <see cref="UpdateRankAsync"/> but <paramref name="displayRow"/> is the 1-based row in the grid (sorted by rank then title) where the moved cluster should appear—use for drag-and-drop instead of rank-slot index.
        /// </summary>
        Task UpdateRankAtDisplayRowAsync(Guid employeeTypeId, int displayRow);

        /// <summary>
        /// Gets all active employee types.
        /// </summary>
        /// <returns>A list of active EmployeeTypeDto.</returns>
        /// <summary>
        /// Validates if an EmployeeTypeID exists in the EmployeeType table and is active.
        /// </summary>
        /// <param name="employeeTypeID">The EmployeeType ID to validate.</param>
        /// <exception cref="SMS.Generic.Common.WebApiException">Thrown when EmployeeType does not exist or is not active.</exception>
        Task ValidateEmployeeTypeExistsAsync(Guid employeeTypeID);

        /// <summary>
        /// Validates if an employee can be promoted from current EmployeeType to a new EmployeeType based on rank.
        /// </summary>
        /// <param name="currentEmployeeTypeID">The current EmployeeType ID (nullable).</param>
        /// <param name="newEmployeeTypeID">The new EmployeeType ID to validate.</param>
        /// <exception cref="SMS.Generic.Common.WebApiException">Thrown when validation fails.</exception>
        Task ValidateRankPromotionAsync(Guid? currentEmployeeTypeID, Guid newEmployeeTypeID);

        /// <summary>
        /// Gets an employee type by name (case-insensitive match on EmployeeTypeLong or EmployeeTypeShort).
        /// </summary>
        /// <param name="name">The employee type name (long or short) to find.</param>
        /// <returns>The employee type DTO if found; otherwise, null.</returns>
        Task<EmployeeTypeDto?> GetByNameAsync(string name,Guid departmentId);

        /// <summary>
        /// Creates the MasterType employee type if it does not exist, or returns the existing ID.
        /// </summary>
        /// <param name="departmentId">The department identifier for the employee type.</param>
        /// <returns>The EmployeeTypeID of the MasterType (existing or newly created).</returns>
        Task<Guid> UpsertMasterTypeAsync(Guid departmentId);

        /// <summary>
        /// Ensures a default LEO employee type exists for the department (case-insensitive short/long).
        /// </summary>
        Task EnsureLeoEmployeeTypeExistsAsync(Guid departmentId);

        /// <summary>
        /// Upserts with optional <see cref="EmployeeTypeUpsertResultDto.AffectedSameRankTypes"/> when same-rank sync runs.
        /// </summary>
        Task<EmployeeTypeUpsertResultDto> UpsertEmployeeTypeWithResultAsync(EmployeeTypeDto dto, bool includeAffectedSameRankTypes, bool isSuperUser = false);
    }
}

