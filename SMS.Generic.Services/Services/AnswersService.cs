using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing Answers operations using the generic service pattern.
    /// </summary>
    public class AnswersService : GenericService<AnswersDto, Answers>, IAnswersService
    {
        private readonly IAnswersRepository _answersRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswersService"/> class.
        /// </summary>
        /// <param name="repository">The answers repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public AnswersService(
            IAnswersRepository repository,
            IMapper mapper,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "AnswerId", tokenParser)
        {
            _answersRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Inserts a new answer or updates an existing one.
        /// </summary>
        /// <param name="dto">The answers DTO to insert or update.</param>
        /// <returns>The upserted answers DTO.</returns>
        public override async Task<AnswersDto> UpsertAsync(AnswersDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate: ResponseId + QuestionId + Answer combination must be unique
            if (dto.ResponseId.HasValue && dto.ResponseId.Value != Guid.Empty &&
                dto.QuestionId.HasValue && dto.QuestionId.Value != Guid.Empty &&
                !string.IsNullOrEmpty(dto.Answer))
            {
                await ValidateDuplicateAsync(
                    propertyName: "Answer",
                    value: dto.Answer,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => 
                        e.ResponseId == dto.ResponseId.Value && 
                        e.QuestionId == dto.QuestionId.Value,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_ANSWER);
            }

            return await base.UpsertAsync(dto);
        }
    }
}
