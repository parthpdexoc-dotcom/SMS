using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class PostDocumentService : GenericService<PostDocumentDto, PostDocument>, IPostDocumentService
    {
        private readonly IPostDocumentRepository _postDocumentRepository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IPostService _postService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDocumentService"/> class.
        /// </summary>
        /// <param name="repository">The department repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PostDocumentService(
            IPostDocumentRepository repository,
            IMapper mapper,
            IPostService postService,
            ILogger<PostDocumentService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "PostDocumentId", tokenParser)
        {
            _postDocumentRepository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _postService = postService;
        }

        public override async Task<PostDocumentDto> UpsertAsync([FromBody] PostDocumentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate DepartmentName (globally unique)
            if (!string.IsNullOrWhiteSpace(dto.DocumentName))
            {

                await ValidateForeignKeyAsync<IPostService, PostDto>(_postService, dto.PostID, "PostId");

                await ValidateDuplicateAsync(
                    propertyName: "PostDocumentId",
                    value: dto.PostDocumentId,
                    dto: dto,
                    isDepartmentScoped: false,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_JOB_DOCUMENT);
            }

            return await base.UpsertAsync(dto);
        }
    }
}
