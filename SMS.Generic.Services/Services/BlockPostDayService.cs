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
    /// Service for managing block post day operations using the generic service pattern.
    /// </summary>
    public class BlockPostDayService : GenericService<BlockPostDayDto, BlockPostDay>, IBlockPostDayService
    {
        private readonly IBlockPostDayRepository _blockPostDayRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPostDayService"/> class.
        /// </summary>
        /// <param name="repository">The block post day repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public BlockPostDayService(
            IBlockPostDayRepository repository,
            IMapper mapper,
            ILogger<BlockPostDayService> logger) : base(repository, mapper, "BlockPostID")
        {
            _blockPostDayRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a block post day by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The block post day DTO if found; otherwise, null.</returns>
        public async Task<BlockPostDayDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _blockPostDayRepository.GetByIdAsync(id, "BlockPostID");
            if (entity == null)
                return null;

            return _mapper.Map<BlockPostDayDto>(entity);
        }
    }
}

