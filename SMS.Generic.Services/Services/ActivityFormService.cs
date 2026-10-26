using System.Linq;
using AutoMapper;
using SMS.Generic.Common.Constants;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing activity form operations (questionnaire with questions and options).
    /// </summary>
    public class ActivityFormService : GenericService<QuestionnaireDto, Questionnaire>, IActivityFormService
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IQuestionService _questionService;
        private readonly IOptionService _optionService;
        private readonly IDepartmentQuestionnaireService _departmentQuestionnaireService;
        private readonly ITokenParser? _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFormService"/> class.
        /// </summary>
        /// <param name="repository">The questionnaire repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="questionnaireService">The questionnaire service (for validated questionnaire upsert).</param>
        /// <param name="questionService">The question service.</param>
        /// <param name="optionService">The option service.</param>
        /// <param name="departmentQuestionnaireService">The department questionnaire service.</param>
        /// <param name="tokenParser">The token parser for current user context.</param>
        public ActivityFormService(
            IQuestionnaireRepository repository,
            IMapper mapper,
            IQuestionnaireService questionnaireService,
            IQuestionService questionService,
            IOptionService optionService,
            IDepartmentQuestionnaireService departmentQuestionnaireService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "QuestionnaireId", tokenParser)
        {
            _questionnaireService = questionnaireService ?? throw new ArgumentNullException(nameof(questionnaireService));
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _optionService = optionService ?? throw new ArgumentNullException(nameof(optionService));
            _departmentQuestionnaireService = departmentQuestionnaireService ?? throw new ArgumentNullException(nameof(departmentQuestionnaireService));
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Gets the complete questionnaire form data including questions and options by questionnaire ID.
        /// </summary>
        /// <param name="questionnaireId">The questionnaire ID.</param>
        /// <returns>The questionnaire form data with questions and options.</returns>
        public async Task<QuestionnaireFormDto?> GetQuestionnaireFormByIdAsync(Guid questionnaireId)
        {
            // Get the questionnaire
            var questionnaire = await GetByIdAsync(questionnaireId);
            if (questionnaire == null)
            {
                return null;
            }

            // Get all questions for this questionnaire
            var questionDtos = (await _questionService.GetAllAsync(
                filter: q => q.QuestionnaireId == questionnaireId,
                includes: null,
                pageParam: null
            )).ToList();

            // Get all options for all questions
            var questionIds = questionDtos.Where(q => q.QuestionId.HasValue).Select(q => q.QuestionId!.Value).ToList();
            var allOptions = new List<OptionDto>();

            if (questionIds.Any())
            {
                allOptions = (await _optionService.GetAllAsync(
                    filter: o => questionIds.Contains(o.QuestionId),
                    includes: null,
                    pageParam: null
                )).ToList();
            }

            // Build the response with questions and their options
            var questionsWithOptions = questionDtos
                .OrderBy(q => q.DisplayOrder)
                .Select(q => new QuestionWithOptionsDto
                {
                    QuestionId = q.QuestionId,
                    QuestionnaireId = q.QuestionnaireId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    IsRequired = q.IsRequired,
                    DisplayOrder = q.DisplayOrder,
                    Options = allOptions
                        .Where(o => o.QuestionId == q.QuestionId)
                        .OrderBy(o => o.DisplayOrder)
                        .ToList()
                })
                .ToList();

            return new QuestionnaireFormDto
            {
                Questionnaire = questionnaire,
                Questions = questionsWithOptions
            };
        }

        /// <summary>
        /// Inserts or updates a complete questionnaire form including questions and options.
        /// </summary>
        /// <param name="formDto">The questionnaire form data to insert or update.</param>
        /// <returns>The upserted questionnaire form data with questions and options.</returns>
        public async Task<QuestionnaireFormDto> UpsertQuestionnaireFormAsync(QuestionnaireFormDto formDto)
        {
            if (formDto == null)
                throw new ArgumentNullException(nameof(formDto));

            if (formDto.Questionnaire == null)
                throw new ArgumentException(ModelValidationErrorCodes.QUESTIONNAIRE_CANNOT_BE_NULL, nameof(formDto));

            // 1. Upsert the questionnaire
            var questionnaire = await _questionnaireService.UpsertAsync(formDto.Questionnaire);
            var questionnaireId = questionnaire.QuestionnaireId!.Value;

            // When user type is Employee and this is a new form, insert into DepartmentQuestionnaire
            var isNewForm = !formDto.Questionnaire.QuestionnaireId.HasValue || formDto.Questionnaire.QuestionnaireId.Value == Guid.Empty;
            var userTypeClaim = _tokenParser?.GetClaimValue(ClaimNames.UserType);
            if (string.Equals(userTypeClaim, "Employee", StringComparison.OrdinalIgnoreCase) && isNewForm)
            {
                var departmentId = _tokenParser.GetDepartmentID();
                if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                {
                    await _departmentQuestionnaireService.UpsertAsync(new DepartmentQuestionnaireDto
                    {
                        DepartmentID = departmentId.Value,
                        QuestionnaireId = questionnaireId
                    });
                }
            }

            // 2. Get existing questions for this questionnaire
            var existingQuestions = (await _questionService.GetAllAsync(
                filter: q => q.QuestionnaireId == questionnaireId,
                includes: null,
                pageParam: null
            )).ToList();
            var existingQuestionIds = existingQuestions
                .Where(q => q.QuestionId.HasValue)
                .Select(q => q.QuestionId!.Value)
                .ToList();

            // 3. Process questions - upsert each question
            var incomingQuestionIds = new List<Guid>();

            if (formDto.Questions != null && formDto.Questions.Any())
            {
                foreach (var questionDto in formDto.Questions)
                {
                    // Set QuestionnaireId for all questions
                    questionDto.QuestionnaireId = questionnaireId;

                    // Upsert the question using service
                    var upsertedQuestion = await _questionService.UpsertAsync(questionDto);

                    if (upsertedQuestion.QuestionId.HasValue)
                    {
                        incomingQuestionIds.Add(upsertedQuestion.QuestionId.Value);
                    }
                }
            }

            // 4. Delete questions that are no longer in the form
            var questionsToDelete = existingQuestionIds.Except(incomingQuestionIds).ToList();
            if (questionsToDelete.Any())
            {
                foreach (var questionId in questionsToDelete)
                {
                    await _questionService.DeleteAsync<Guid>(questionId);
                }
            }

            // 5. Get all questions after upsert to get the actual QuestionIds
            var allQuestions = (await _questionService.GetAllAsync(
                filter: q => q.QuestionnaireId == questionnaireId,
                includes: null,
                pageParam: null
            )).ToList();

            // 6. Process options for each question
            if (formDto.Questions != null)
            {
                foreach (var questionDto in formDto.Questions)
                {
                    // Find the actual QuestionId after upsert.
                    // Treat default/empty Guid as "no id" - match by QuestionText and DisplayOrder in that case.
                    var hasValidQuestionId = questionDto.QuestionId.HasValue && questionDto.QuestionId.Value != Guid.Empty;
                    var actualQuestion = allQuestions.FirstOrDefault(q =>
                        (hasValidQuestionId && q.QuestionId == questionDto.QuestionId!.Value) ||
                        (!hasValidQuestionId &&
                         q.QuestionText == questionDto.QuestionText &&
                         q.DisplayOrder == questionDto.DisplayOrder));

                    if (actualQuestion == null || !actualQuestion.QuestionId.HasValue)
                        continue;

                    var actualQuestionId = actualQuestion.QuestionId.Value;

                    // Get existing options for this question
                    var existingOptions = (await _optionService.GetAllAsync(
                        filter: o => o.QuestionId == actualQuestionId,
                        includes: null,
                        pageParam: null
                    )).ToList();
                    var existingOptionIds = existingOptions
                        .Where(o => o.OptionId.HasValue)
                        .Select(o => o.OptionId!.Value)
                        .ToList();

                    // Process options - upsert each option
                    var incomingOptionIds = new List<Guid>();

                    if (questionDto.Options != null && questionDto.Options.Any())
                    {
                        foreach (var optionDto in questionDto.Options)
                        {
                            // Set QuestionId for all options
                            optionDto.QuestionId = actualQuestionId;

                            // Upsert the option using service
                            var upsertedOption = await _optionService.UpsertAsync(optionDto);

                            if (upsertedOption.OptionId.HasValue)
                            {
                                incomingOptionIds.Add(upsertedOption.OptionId.Value);
                            }
                        }
                    }

                    // Delete options that are no longer in the form
                    var optionsToDelete = existingOptionIds.Except(incomingOptionIds).ToList();
                    if (optionsToDelete.Any())
                    {
                        foreach (var optionId in optionsToDelete)
                        {
                            await _optionService.DeleteAsync<Guid>(optionId);
                        }
                    }
                }
            }

            // 7. Return the complete form data
            return await GetQuestionnaireFormByIdAsync(questionnaireId) ?? 
                throw new InvalidOperationException(ModelValidationErrorCodes.FAILED_TO_RETRIEVE_UPSERTED_QUESTIONNAIRE_FORM);
        }

        /// <inheritdoc />
        public async Task EnrichQuestionnaireSubmissionsAsync(
            List<QuestionnaireSubmissionDto> submissions,
            Dictionary<Guid, List<SMS.Generic.Dtos.Dtos.AnswerItemInternal>> submissionInternalData)
        {
            if (submissions.Count == 0) return;

            var questionnaireIds = submissions
                .Where(s => s.QuestionnaireId.HasValue && s.QuestionnaireId.Value != Guid.Empty)
                .Select(s => s.QuestionnaireId!.Value)
                .Distinct()
                .ToList();

            var optionIdSet = new HashSet<Guid>();
            foreach (var items in submissionInternalData.Values)
                foreach (var item in items)
                    if (item.SelectedOptionIds != null)
                        foreach (var id in item.SelectedOptionIds)
                            optionIdSet.Add(id);

            var questionIds = submissions
                .SelectMany(s => s.Answers ?? Enumerable.Empty<QuestionnaireAnswerItemDto>())
                .Where(a => a.QuestionId.HasValue && a.QuestionId.Value != Guid.Empty)
                .Select(a => a.QuestionId!.Value)
                .Distinct()
                .ToList();

            var questionnaireById = questionnaireIds.Count > 0
                ? (await _questionnaireService.GetAllAsync(q => questionnaireIds.Contains(q.QuestionnaireId), null, null)
                    .ConfigureAwait(false))
                    ?.Where(q => q.QuestionnaireId.HasValue)
                    .ToDictionary(q => q.QuestionnaireId!.Value, q => q) ?? new()
                : new();

            var questionById = questionIds.Count > 0
                ? (await _questionService.GetAllAsync(q => questionIds.Contains(q.QuestionId), null, null)
                    .ConfigureAwait(false))
                    ?.Where(q => q.QuestionId.HasValue)
                    .ToDictionary(q => q.QuestionId!.Value, q => q) ?? new()
                : new();

            var optionById = optionIdSet.Count > 0
                ? (await _optionService.GetAllAsync(o => optionIdSet.ToList().Contains(o.OptionId), null, null)
                    .ConfigureAwait(false))
                    ?.Where(o => o.OptionId.HasValue)
                    .ToDictionary(o => o.OptionId!.Value, o => o) ?? new()
                : new();

            foreach (var submission in submissions)
            {
                if (submission.QuestionnaireId.HasValue && questionnaireById.TryGetValue(submission.QuestionnaireId.Value, out var qnDto))
                    submission.QuestionnaireTitle = qnDto.Title;

                if (!submission.QuestionnaireId.HasValue || !submissionInternalData.TryGetValue(submission.QuestionnaireId.Value, out var internalItems))
                    continue;

                var internalByQuestionId = internalItems.Where(i => i.QuestionId.HasValue).ToDictionary(i => i.QuestionId!.Value, i => i);

                foreach (var answer in submission.Answers ?? Enumerable.Empty<QuestionnaireAnswerItemDto>())
                {
                    if (answer.QuestionId.HasValue && questionById.TryGetValue(answer.QuestionId.Value, out var quDto))
                        answer.QuestionText = quDto.QuestionText;

                    if (answer.QuestionId.HasValue && internalByQuestionId.TryGetValue(answer.QuestionId.Value, out var internalItem)
                        && internalItem.SelectedOptionIds?.Count > 0)
                    {
                        var parts = internalItem.SelectedOptionIds
                            .Select(optionId => optionById.TryGetValue(optionId, out var opt) && !string.IsNullOrEmpty(opt.OptionText) ? opt.OptionText! : optionId.ToString())
                            .ToList();
                        answer.Answer = string.Join(", ", parts);
                    }
                }
            }
        }
    }
}
