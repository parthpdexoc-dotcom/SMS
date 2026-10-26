using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing division operations using the generic service pattern.
    /// </summary>
    public class DivisionService : GenericService<DivisionDto, Division>, IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionService"/> class.
        /// </summary>
        /// <param name="repository">The division repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public DivisionService(
            IDivisionRepository repository,
            IMapper mapper,
            ILogger<DivisionService> logger) : base(repository, mapper, "DivisionID")
        {
            _divisionRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a division by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The division DTO if found; otherwise, null.</returns>
        public async Task<DivisionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _divisionRepository.GetByIdAsync(id, "DivisionID");
            if (entity == null)
                return null;

            return _mapper.Map<DivisionDto>(entity);
        }

        /// <summary>
        /// Creates or updates a division.
        /// </summary>
        public override async Task<DivisionDto> UpsertAsync(DivisionDto dto, bool isSuperUser = false)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return await base.UpsertAsync(dto, isSuperUser);
        }
    }
}



