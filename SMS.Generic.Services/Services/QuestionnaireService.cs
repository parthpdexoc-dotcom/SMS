using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing Questionnaire operations using the generic service pattern.
    /// </summary>
    public class QuestionnaireService : GenericService<QuestionnaireDto, Questionnaire>, IQuestionnaireService
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireService"/> class.
        /// </summary>
        /// <param name="repository">The questionnaire repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="employeeService">The employee service for foreign key validation.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public QuestionnaireService(
            IQuestionnaireRepository repository,
            IMapper mapper,
            IEmployeeService employeeService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "QuestionnaireId", tokenParser)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Inserts a new questionnaire or updates an existing one.
        /// </summary>
        /// <param name="dto">The questionnaire DTO to insert or update.</param>
        /// <returns>The upserted questionnaire DTO.</returns>
        public override async Task<QuestionnaireDto> UpsertAsync(QuestionnaireDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // When creating/updating a source questionnaire (SourceQuestionnaireId is null), validate Title is not duplicate among other source questionnaires
            if (!dto.SourceQuestionnaireId.HasValue && !string.IsNullOrWhiteSpace(dto.Title))
            {
                await ValidateDuplicateAsync(
                    propertyName: "Title",
                    value: dto.Title,
                    dto: dto,
                    additionalFilter: e => e.SourceQuestionnaireId == null,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_QUESTIONNAIRE_TITLE);
            }

            // Validate SourceQuestionnaireId exists if provided
            if (dto.SourceQuestionnaireId.HasValue)
            {
                await ValidateForeignKeyAsync<IQuestionnaireService, QuestionnaireDto>(
                    this,
                    dto.SourceQuestionnaireId.Value,
                    "Questionnaire");
            }

            return await base.UpsertAsync(dto);
        }
    }
}
