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
    /// Service for managing posts on duty operations using the generic service pattern.
    /// </summary>
    public class PostsOnDutyService : GenericService<PostsOnDutyDto, PostsOnDuty>, IPostsOnDutyService
    {
        private readonly IPostsOnDutyRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsOnDutyService"/> class.
        /// </summary>
        /// <param name="repository">The posts on duty repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PostsOnDutyService(
            IPostsOnDutyRepository repository,
            IMapper mapper,
            ILogger<PostsOnDutyService> logger) : base(repository, mapper, "PostsOnDutyID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a posts on duty by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The posts on duty DTO if found; otherwise, null.</returns>
        public async Task<PostsOnDutyDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "PostsOnDutyID");
            if (entity == null)
                return null;

            return _mapper.Map<PostsOnDutyDto>(entity);
        }
    }
}

