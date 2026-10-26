using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee-specific repository implementation.
    /// </summary>
    public class EmployeeRepository : GenericRepository<Employee, SMSDbContext>, IEmployeeRepository
    {
        private readonly SMSDbContext _context;
        private readonly DbSet<Employee> _dbSet;
        private readonly ILogger<EmployeeRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeRepository(
            SMSDbContext context,
            ILogger<EmployeeRepository> logger) : base(context, logger)
        {
            _context = context;
            _dbSet = context.Set<Employee>();
            _logger = logger;
        }

        /// <summary>
        /// Retrieves an employee by employee number (mapped to EmployeeBadgeNumber in the new schema).
        /// </summary>
        /// <param name="employeeNo">The employee number / badge number.</param>
        /// <returns>The employee if found; otherwise, null.</returns>
        public async Task<Employee?> GetByEmployeeNoAsync(string employeeNo)
        {
            if (string.IsNullOrWhiteSpace(employeeNo))
                return null;

            // In the new Employee schema, we no longer have EmployeeNo; use EmployeeBadgeNumber instead.
            var employees = await GetAllAsync(e => e.BadgeNumber == employeeNo);
            return employees.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an employee with all related details (Department, EmployeeType, EmployeeStatus, AdminTypes) for login operations.
        /// Uses a single optimized query with LINQ joins to fetch all required data.
        /// </summary>
        /// <param name="departmentCode">The department code from Department table.</param>
        /// <param name="username">The username of the employee.</param>
        /// <returns>The employee login information DTO if found; otherwise, null.</returns>
        public async Task<EmployeeLoginInfoDto?> GetEmployeeWithDetailsForLoginAsync(string departmentCode, string username)
        {
            if (string.IsNullOrWhiteSpace(departmentCode) || string.IsNullOrWhiteSpace(username))
                return null;

            var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);

            // Single LINQ query with all includes and projection
            var employeeInfo = await _context.Employee
                .AsSplitQuery()
                .Where(e =>
                    e.Username != null &&
                    e.Department != null &&
                    e.Department.DepartmentCode != null &&
                    e.Username.Trim().ToLower() == username.Trim().ToLower() &&
                    e.Department.DepartmentCode.Trim().ToLower() == departmentCode.Trim().ToLower())
                .Include(e => e.Department)
                .Include(e => e.EmployeeType)
                .Include(e => e.Status)
                .Include(e => e.EmployeeAdmins)
                    .ThenInclude(ea => ea.AdminType)
                .Include(e => e.DepartmentAdmins)
                    .ThenInclude(da => da.AdminType)
                .Select(e => new EmployeeLoginInfoDto
                {
                    // Employee Info
                    EmployeeID = e.EmployeeID,
                    Username = e.Username ?? string.Empty,
                    BadgeNumber = e.BadgeNumber,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Password = e.Password, // For validation
                    PrimaryEmail = e.PrimaryEmail,
                    SecondaryEmail = e.SecondaryEmail,

                    // Department Info
                    DepartmentID = e.DepartmentID,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                    DepartmentCode = e.Department != null ? e.Department.DepartmentCode : null,

                    // EmployeeType Info
                    EmployeeTypeID = e.EmployeeTypeID,
                    EmployeeTypeShort = e.EmployeeType != null ? e.EmployeeType.EmployeeTypeShort : null,
                    EmployeeTypeLong = e.EmployeeType != null ? e.EmployeeType.EmployeeTypeLong : null,
                    IsSupervisor = e.EmployeeType != null && e.EmployeeType.IsSupervisor,
                    IsSystemUser = e.EmployeeType != null && e.EmployeeType.IsSystemUser,

                    // EmployeeStatus Info
                    StatusID = e.StatusID,
                    StatusShort = e.Status != null ? e.Status.StatusShort : null,
                    StatusLong = e.Status != null ? e.Status.StatusLong : null,

                    // Employee approval
                    IsApproved = e.IsApproved,
                    IsPasswordReset = e.IsPasswordReset,

                    // AdminTypes from EmployeeAdmins and DepartmentAdmins (deduplicated by AdminTypeID)
                    AdminTypes = e.EmployeeAdmins
                        .Where(ea => ea.AdminType != null)
                        .Select(ea => new AdminTypeInfoDto
                        {
                            AdminTypeID = ea.AdminTypeID,
                            AdminTypeShort = ea.AdminType!.AdminTypeShort,
                            AdminTypeLong = ea.AdminType.AdminTypeLong
                        })
                        .Union(e.DepartmentAdmins
                            .Where(da => da.AdminType != null)
                            .Select(da => new AdminTypeInfoDto
                            {
                                AdminTypeID = da.AdminTypeID,
                                AdminTypeShort = da.AdminType!.AdminTypeShort,
                                AdminTypeLong = da.AdminType.AdminTypeLong
                            }))
                        .GroupBy(x => x.AdminTypeID)
                        .Select(g => g.First())
                        .ToList(),

                    HasActiveSuspendedPeriod = _context.EmployeeSkill.Any(es =>
                        es.EmployeeID == e.EmployeeID &&
                        es.Skill != null &&
                        ((es.Skill.SkillShort != null && es.Skill.SkillShort.ToLower() == "suspended") ||
                         (es.Skill.SkillLong != null && es.Skill.SkillLong.ToLower() == "suspended")) &&
                        es.StartDate <= utcToday &&
                        (!es.EndDate.HasValue || es.EndDate.Value >= utcToday)),

                    RestrictWebLoginForNonAdminEmployee = _context.DepartmentPolicy.Any(p =>
                        p.DepartmentID == e.DepartmentID && p.RestrictWebLoginForNonAdminEmployee),

                    EnableNormalEmployeeOneSession = _context.DepartmentConfiguration.Any(c =>
                        c.DepartmentID == e.DepartmentID && c.EnableNormalEmployeeOneSession)
                })
                .FirstOrDefaultAsync();

            return employeeInfo;
        }

        /// <summary>
        /// Retrieves an employee with all related details for impersonation token generation.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>The employee login information DTO if found; otherwise, null.</returns>
        public async Task<EmployeeLoginInfoDto?> GetEmployeeWithDetailsByIdAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                return null;

            var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);

            var employeeInfo = await _context.Employee
                .AsSplitQuery()
                .Where(e => e.EmployeeID == employeeId)
                .Include(e => e.Department)
                .Include(e => e.EmployeeType)
                .Include(e => e.Status)
                .Include(e => e.EmployeeAdmins)
                    .ThenInclude(ea => ea.AdminType)
                .Include(e => e.DepartmentAdmins)
                    .ThenInclude(da => da.AdminType)
                .Select(e => new EmployeeLoginInfoDto
                {
                    EmployeeID = e.EmployeeID,
                    Username = e.Username ?? string.Empty,
                    BadgeNumber = e.BadgeNumber,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Password = e.Password,
                    PrimaryEmail = e.PrimaryEmail,
                    SecondaryEmail = e.SecondaryEmail,

                    DepartmentID = e.DepartmentID,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                    DepartmentCode = e.Department != null ? e.Department.DepartmentCode : null,

                    EmployeeTypeID = e.EmployeeTypeID,
                    EmployeeTypeShort = e.EmployeeType != null ? e.EmployeeType.EmployeeTypeShort : null,
                    EmployeeTypeLong = e.EmployeeType != null ? e.EmployeeType.EmployeeTypeLong : null,
                    IsSupervisor = e.EmployeeType != null && e.EmployeeType.IsSupervisor,
                    IsSystemUser = e.EmployeeType != null && e.EmployeeType.IsSystemUser,

                    StatusID = e.StatusID,
                    StatusShort = e.Status != null ? e.Status.StatusShort : null,
                    StatusLong = e.Status != null ? e.Status.StatusLong : null,

                    IsApproved = e.IsApproved,
                    IsPasswordReset = e.IsPasswordReset,

                    AdminTypes = e.EmployeeAdmins
                        .Where(ea => ea.AdminType != null)
                        .Select(ea => new AdminTypeInfoDto
                        {
                            AdminTypeID = ea.AdminTypeID,
                            AdminTypeShort = ea.AdminType!.AdminTypeShort,
                            AdminTypeLong = ea.AdminType.AdminTypeLong
                        })
                        .Union(e.DepartmentAdmins
                            .Where(da => da.AdminType != null)
                            .Select(da => new AdminTypeInfoDto
                            {
                                AdminTypeID = da.AdminTypeID,
                                AdminTypeShort = da.AdminType!.AdminTypeShort,
                                AdminTypeLong = da.AdminType.AdminTypeLong
                            }))
                        .GroupBy(x => x.AdminTypeID)
                        .Select(g => g.First())
                        .ToList(),

                    HasActiveSuspendedPeriod = _context.EmployeeSkill.Any(es =>
                        es.EmployeeID == e.EmployeeID &&
                        es.Skill != null &&
                        ((es.Skill.SkillShort != null && es.Skill.SkillShort.ToLower() == "suspended") ||
                         (es.Skill.SkillLong != null && es.Skill.SkillLong.ToLower() == "suspended")) &&
                        es.StartDate <= utcToday &&
                        (!es.EndDate.HasValue || es.EndDate.Value >= utcToday))
                })
                .FirstOrDefaultAsync();

            return employeeInfo;
        }

        /// <summary>
        /// Retrieves an employee by ID including employee skills and nested skill details.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>The employee entity if found; otherwise, null.</returns>
        public async Task<Employee?> GetByIdWithSkillsAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                return null;

            return await _context.Employee
                .AsSplitQuery()
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);
        }

        /// <summary>
        /// Updates the password for an employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="hashedPassword">The hashed password to set.</param>
        /// <returns>True if the password was updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdatePasswordAsync(Guid employeeId, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                var employee = await _context.Employee.FindAsync(employeeId);
                if (employee == null)
                    return false;

                employee.Password = hashedPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update password for EmployeeId {EmployeeId}", employeeId);
                return false;
            }
        }

        public async Task<bool> SetActiveSessionIdAsync(Guid employeeId, Guid sessionId)
        {
            var updated = await _dbSet
                .Where(e => e.EmployeeID == employeeId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.ActiveSessionId, sessionId))
                .ConfigureAwait(false);
            return updated > 0;
        }

        public Task<bool> IsActiveSessionAsync(Guid employeeId, Guid sessionId) =>
            _dbSet.AnyAsync(e => e.EmployeeID == employeeId && e.ActiveSessionId == sessionId);

        /// <inheritdoc />
        public Task<Employee?> GetByIdForFlowAuditAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            _context.Employee
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeID == employeeId, cancellationToken);
    }
}

