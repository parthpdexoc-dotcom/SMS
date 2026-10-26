using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing role operations using the generic service pattern.
    /// </summary>
    public class RoleService : GenericService<RoleDto, Role>, IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="repository">The role repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RoleService(
            IRoleRepository repository,
            IMapper mapper,
            ILogger<RoleService> logger) : base(repository, mapper, "ID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a role by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The role DTO if found; otherwise, null.</returns>
        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ID");
            if (entity == null)
                return null;

            return _mapper.Map<RoleDto>(entity);
        }
    }
}

