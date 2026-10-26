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
    /// Service for managing post type operations using the generic service pattern.
    /// </summary>
    public class PostTypeService : GenericService<PostTypeDto, PostType>, IPostTypeService
    {
        private readonly IPostTypeRepository _postTypeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostTypeService"/> class.
        /// </summary>
        /// <param name="repository">The post type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PostTypeService(
            IPostTypeRepository repository,
            IMapper mapper,
            ILogger<PostTypeService> logger) : base(repository, mapper, "PostTypeID")
        {
            _postTypeRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a post type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The post type DTO if found; otherwise, null.</returns>
        public async Task<PostTypeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _postTypeRepository.GetByIdAsync(id, "PostTypeID");
            if (entity == null)
                return null;

            return _mapper.Map<PostTypeDto>(entity);
        }
    }
}

