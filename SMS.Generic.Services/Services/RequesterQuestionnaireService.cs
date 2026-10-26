using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing RequesterQuestionnaire operations using the generic service pattern.
    /// </summary>
    public class RequesterQuestionnaireService : GenericService<RequesterQuestionnaireDto, RequesterQuestionnaire>, IRequesterQuestionnaireService
    {
        private readonly IRequesterQuestionnaireRepository _repository;
        private readonly IMapper _mapper;
        private readonly IRequesterService _requesterService;
        private readonly IQuestionnaireService _questionnaireService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterQuestionnaireService"/> class.
        /// </summary>
        /// <param name="repository">The RequesterQuestionnaire repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="requesterService">The requester service for foreign key validation.</param>
        /// <param name="questionnaireService">The questionnaire service for foreign key validation.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public RequesterQuestionnaireService(
            IRequesterQuestionnaireRepository repository,
            IMapper mapper,
            IRequesterService requesterService,
            IQuestionnaireService questionnaireService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "RequesterQuestionnaireId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _requesterService = requesterService;
            _questionnaireService = questionnaireService;
        }

        /// <summary>
        /// Inserts a new RequesterQuestionnaire or updates an existing one.
        /// </summary>
        /// <param name="dto">The RequesterQuestionnaire DTO to insert or update.</param>
        /// <returns>The upserted RequesterQuestionnaire DTO.</returns>
        public override async Task<RequesterQuestionnaireDto> UpsertAsync(RequesterQuestionnaireDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate RequesterId foreign key exists
            await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterId, "Requester");

            // Validate QuestionnaireId foreign key exists
            await ValidateForeignKeyAsync<IQuestionnaireService, QuestionnaireDto>(_questionnaireService, dto.QuestionnaireId, "Questionnaire");

            // Validate for duplicate RequesterId + QuestionnaireId combination
            // FluentValidation ensures RequesterId and QuestionnaireId are not null at this point
            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.RequesterId == dto.RequesterId.Value && e.QuestionnaireId == dto.QuestionnaireId.Value,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUESTER_QUESTIONNAIRE);

            return await base.UpsertAsync(dto);
        }
    }
}
