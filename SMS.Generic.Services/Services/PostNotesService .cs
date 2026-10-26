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
    /// Service for managing post operations using the generic service pattern.
    /// </summary>
    public class PostNotesService : GenericService<PostNotesDto, PostNotes>, IPostNoteService
    {
        private readonly IPostNotesRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IPostService _postService;
        private readonly ITokenParser _tokenParser;
        /// <summary>
        /// Initializes a new instance of the <see cref="PostService"/> class.
        /// </summary>
        /// <param name="repository">The post repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="skillService">The skill service (used to validate SkillIDs).</param>
        public PostNotesService(
            IPostNotesRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<PostService> logger,
            IPostService postService) : base(repository, mapper, "NoteId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _postService = postService;
            _tokenParser = tokenParser;
        }
        public override async Task<PostNotesDto> UpsertAsync(PostNotesDto dto, bool isSuperUser = false)
        {
            await ValidateForeignKeyAsync<IPostService, PostDto>(_postService, dto.PostID, "Post");

            await ValidateCompositeKeyDuplicateAsync(
                    dto: dto,
                    filterExpression: e => e.PostID == dto.PostID && e.Notes == dto.Notes && e.Comments == dto.Comments,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_JOB_NOTES);
            // Set IsLatest to false for all existing notes with the same PostID (excluding the current note if updating)
            Expression<Func<PostNotes, bool>> filter = dto.NoteId == Guid.Empty 
                ? (Expression<Func<PostNotes, bool>>)(x => x.PostID == dto.PostID)
                : (Expression<Func<PostNotes, bool>>)(x => x.PostID == dto.PostID && x.NoteId != dto.NoteId);
            
            await UpdateAsync(filter, entity => entity.IsLatest = false);
            
            // Set the new/updated note's IsLatest to true before upserting
            dto.IsLatest = true;

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets the latest note for a specific post ID where IsLatest is true.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>The latest post note if found; otherwise, null.</returns>
        public async Task<PostNotesDto?> GetLatestNoteByPostIdAsync(Guid postId)
        {
            var notes = await GetAllAsync(x => x.PostID == postId && x.IsLatest == true);
            return notes.FirstOrDefault();
        }
    }
}

