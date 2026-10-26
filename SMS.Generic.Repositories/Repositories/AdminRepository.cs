using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Admin-specific repository implementation.
    /// </summary>
    public class AdminRepository : GenericRepository<Admin, SMSDbContext>, IAdminRepository
    {
        private readonly SMSDbContext _context;
        private readonly ILogger<AdminRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public AdminRepository(
            SMSDbContext context,
            ILogger<AdminRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves an admin with all related details (AdminType) for login operations.
        /// Uses a single optimized query with LINQ joins to fetch all required data.
        /// </summary>
        /// <param name="username">The username of the admin.</param>
        /// <returns>The admin login information DTO if found; otherwise, null.</returns>
        public async Task<AdminLoginInfoDto?> GetAdminWithDetailsForLoginAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            // Single LINQ query with all includes and projection
            var adminInfo = await _context.Admin
                .Where(a => a.UserName == username)
                .Include(a => a.AdminType)
                .Select(a => new AdminLoginInfoDto
                {
                    // Admin Info
                    AdminID = a.AdminID,
                    UserName = a.UserName ?? string.Empty,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Password = a.Password, // For validation
                    Status = a.Status,

                    // AdminType Info
                    AdminTypeID = a.AdminTypeID,
                    AdminTypeShort = a.AdminType != null ? a.AdminType.AdminTypeShort : null,
                    AdminTypeLong = a.AdminType != null ? a.AdminType.AdminTypeLong : null,
                    IsSuperUser = a.AdminType != null && a.AdminType.IsSuperUser
                })
                .FirstOrDefaultAsync();

            return adminInfo;
        }

        /// <inheritdoc />
        public async Task<AdminLoginInfoDto?> GetAdminWithDetailsByIdAsync(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return null;

            return await _context.Admin
                .Where(admin => admin.AdminID == adminId)
                .Include(admin => admin.AdminType)
                .Select(admin => new AdminLoginInfoDto
                {
                    AdminID = admin.AdminID,
                    UserName = admin.UserName ?? string.Empty,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Status = admin.Status,
                    AdminTypeID = admin.AdminTypeID,
                    AdminTypeShort = admin.AdminType != null ? admin.AdminType.AdminTypeShort : null,
                    AdminTypeLong = admin.AdminType != null ? admin.AdminType.AdminTypeLong : null,
                    IsSuperUser = admin.AdminType != null && admin.AdminType.IsSuperUser
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the password for an admin.
        /// </summary>
        /// <param name="adminId">The admin ID.</param>
        /// <param name="hashedPassword">The hashed password to set.</param>
        /// <returns>True if the password was updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdatePasswordAsync(Guid adminId, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                var admin = await _context.Admin.FindAsync(adminId);
                if (admin == null)
                    return false;

                admin.Password = hashedPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update password for AdminId {AdminId}", adminId);
                return false;
            }
        }

        /// <summary>
        /// Retrieves an admin by username.
        /// </summary>
        /// <param name="username">The username of the admin.</param>
        /// <returns>The admin entity if found; otherwise, null.</returns>
        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Admin
                .FirstOrDefaultAsync(a => a.UserName == username);
        }
    }
}

