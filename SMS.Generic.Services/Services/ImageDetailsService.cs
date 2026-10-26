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
    /// Service for managing image details operations using the generic service pattern.
    /// </summary>
    public class ImageDetailsService : GenericService<ImageDetailsDto, ImageDetails>, IImageDetailsService
    {
        private readonly IImageDetailsRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDetailsService"/> class.
        /// </summary>
        /// <param name="repository">The image details repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ImageDetailsService(
            IImageDetailsRepository repository,
            IMapper mapper,
            ILogger<ImageDetailsService> logger) : base(repository, mapper, "ImageID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an image detail by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The image detail DTO if found; otherwise, null.</returns>
        public async Task<ImageDetailsDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "ImageID");
            if (entity == null)
                return null;

            return _mapper.Map<ImageDetailsDto>(entity);
        }
    }
}

