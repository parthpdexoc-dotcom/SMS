using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for question operations.
    /// </summary>
    public class QuestionService : GenericService<QuestionDto, Question>, IQuestionService
    {
        private readonly IMapper _mapper;
        private readonly IQuestionRepository _repository;
        private readonly IQuestionnaireService _questionnaireService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionService"/> class.
        /// </summary>
        /// <param name="repository">The question repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="questionnaireService">The questionnaire service for foreign key validation.</param>
        /// <param name="tokenParser">The token parser instance (optional).</param>
        public QuestionService(
            IQuestionRepository repository,
            IMapper mapper,
            IQuestionnaireService questionnaireService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "QuestionId", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _questionnaireService = questionnaireService;
        }

        /// <summary>
        /// Inserts a new question or updates an existing one.
        /// </summary>
        /// <param name="dto">The question DTO to insert or update.</param>
        /// <returns>The upserted question DTO.</returns>
        public override async Task<QuestionDto> UpsertAsync(QuestionDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate QuestionnaireId foreign key exists
            if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId.Value != Guid.Empty)
            {
                await ValidateForeignKeyAsync<IQuestionnaireService, QuestionnaireDto>(
                    _questionnaireService,
                    dto.QuestionnaireId.Value,
                    "Questionnaire");
            }

            // Validate for duplicate QuestionText within the same Questionnaire
            if (!string.IsNullOrWhiteSpace(dto.QuestionText) && dto.QuestionnaireId.HasValue)
            {   
                await ValidateDuplicateAsync(
                    propertyName: "QuestionText",
                    value: dto.QuestionText,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => e.QuestionnaireId == dto.QuestionnaireId.Value,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_QUESTION_QUESTION_TEXT);
            }

            // Validate for duplicate DisplayOrder within the same Questionnaire
            if (dto.DisplayOrder > 0 && dto.QuestionnaireId.HasValue)
            {
                await ValidateDuplicateAsync(
                    propertyName: "DisplayOrder",
                    value: dto.DisplayOrder,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => e.QuestionnaireId == dto.QuestionnaireId.Value,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_QUESTION_DISPLAY_ORDER);
            }

            return await base.UpsertAsync(dto);
        }
    }
}
