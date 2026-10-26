using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing request operations using the generic service pattern.
    /// </summary>
    public class RequestNotesService : GenericService<RequestNotesDto, RequestNotes>, IRequestNoteService
    {
        private readonly IRequestNotesRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IRequestService _requestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestService"/> class.
        /// </summary>
        /// <param name="repository">The request repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="requesterService">The requester service (used to validate RequesterID).</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="skillService">The skill service (used to validate SkillIDs).</param>
        public RequestNotesService(
            IRequestNotesRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequestNotesService> logger,
            IRequestService requestService) : base(repository, mapper, "NoteId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _requestService = requestService;
        }

        /// <summary>
        /// Inserts a new request or updates an existing one.
        /// </summary>
        /// <param name="dto">The request DTO to insert or update.</param>
        /// <returns>The upserted request DTO.</returns>
        public override async Task<RequestNotesDto> UpsertAsync(RequestNotesDto dto, bool isSuperUser = false)
        {
            await ValidateForeignKeyAsync<IRequestService, RequestDto>(_requestService, dto.RequestId, "Request");

            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.RequestId == dto.RequestId && e.Notes == dto.Notes && e.Comments == dto.Comments,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUEST_NOTES);

            // Set IsLatest to false for all existing notes with the same RequestId (excluding the current note if updating)
            Expression<Func<RequestNotes, bool>> filter = dto.NoteId == Guid.Empty
                ? (Expression<Func<RequestNotes, bool>>)(x => x.RequestId == dto.RequestId)
                : (Expression<Func<RequestNotes, bool>>)(x => x.RequestId == dto.RequestId && x.NoteId != dto.NoteId);

            await UpdateAsync(filter, entity => entity.IsLatest = false);

            // Set the new/updated note's IsLatest to true before upserting
            dto.IsLatest = true;

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets the latest note for a specific request ID where IsLatest is true.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>The latest request note if found; otherwise, null.</returns>
        public async Task<RequestNotesDto?> GetLatestNoteByRequestIdAsync(Guid requestId)
        {
            var notes = await GetAllAsync(x => x.RequestId == requestId && x.IsLatest == true);
            return notes.FirstOrDefault();
        }
    }
}

