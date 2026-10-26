using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department admin operations using the generic service pattern.
    /// </summary>
    public class DepartmentAdminService : GenericService<DepartmentAdminDto, DepartmentAdmin>, IDepartmentAdminService
    {
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentAdminService"/> class.
        /// </summary>
        /// <param name="repository">The department admin repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public DepartmentAdminService(
            IDepartmentAdminRepository repository,
            IMapper mapper,
            ILogger<DepartmentAdminService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "DepartmentAdminID", tokenParser)
        {
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new DepartmentAdminDto or updates an existing one with duplicate validation.
        /// DepartmentID may be sent in the payload; when null or empty, it is taken from the current user's token.
        /// </summary>
        /// <param name="dto">The DepartmentAdminDto to insert or update.</param>
        /// <param name="isSuperUser">
        /// When true, bypasses automatic audit field population (Created/Edited tracking).
        /// Defaults to false.
        /// </param>
        /// <returns>The upserted DepartmentAdminDto.</returns>
        public override async Task<DepartmentAdminDto> UpsertAsync(DepartmentAdminDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (dto.DepartmentID == Guid.Empty)
            {
                var departmentIdFromToken = _tokenParser.GetDepartmentID();
                if (departmentIdFromToken.HasValue && departmentIdFromToken.Value != Guid.Empty)
                {
                    dto.DepartmentID = departmentIdFromToken.Value;
                }
            }

            return await base.UpsertAsync(dto, isSuperUser);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> GetEmployeeIdsByDepartmentAndAdminTypeAsync(Guid departmentId, Guid adminTypeId)
        {
            Expression<Func<DepartmentAdmin, bool>> filter = da =>
                da.DepartmentID == departmentId && da.AdminTypeID == adminTypeId;
            var items = await GetAllAsync(filter);
            return items.Select(dto => dto.EmployeeID).Distinct().ToList();
        }
    }
}
