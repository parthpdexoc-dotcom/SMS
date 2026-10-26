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
    /// Service for managing users operations using the generic service pattern.
    /// </summary>
    public class UsersService : GenericService<UsersDto, Users>, IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="repository">The users repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public UsersService(
            IUsersRepository repository,
            IMapper mapper,
            ILogger<UsersService> logger) : base(repository, mapper, "Id")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a user by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        public async Task<UsersDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "Id");
            if (entity == null)
                return null;

            return _mapper.Map<UsersDto>(entity);
        }
    }
}

