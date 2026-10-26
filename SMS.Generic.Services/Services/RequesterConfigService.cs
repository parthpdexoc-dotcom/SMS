using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing requester config operations using the generic service pattern.
    /// </summary>
    public class RequesterConfigService : GenericService<RequesterConfigDto, RequesterConfig>, IRequesterConfigService
    {
        private readonly IRequesterConfigRepository _repository;
        private readonly IRequesterService _requesterService;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigService"/> class.
        /// </summary>
        /// <param name="repository">The requester config repository.</param>
        /// <param name="requesterService">The requester service for validation.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterConfigService(
            IRequesterConfigRepository repository,
            IRequesterService requesterService,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequesterConfigService> logger) : base(repository, mapper, "RequesterConfigID", tokenParser)
        {
            _repository = repository;
            _requesterService = requesterService;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new requester config or updates an existing one.
        /// </summary>
        /// <param name="dto">The requester config DTO to insert or update.</param>
        /// <returns>The upserted requester config DTO.</returns>
        public override async Task<RequesterConfigDto> UpsertAsync(RequesterConfigDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate RequesterID exists if provided
            await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterID, "Requester");

            return await base.UpsertAsync(dto);
        }
    }
}
