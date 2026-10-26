using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing DepartmentQuestionnaire operations using the generic service pattern.
    /// </summary>
    public class DepartmentQuestionnaireService : GenericService<DepartmentQuestionnaireDto, DepartmentQuestionnaire>, IDepartmentQuestionnaireService
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IMapper _mapper;
        private readonly ITokenParser? _tokenParser;
        private readonly IDepartmentQuestionnaireRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentQuestionnaireService"/> class.
        /// </summary>
        /// <param name="repository">The department questionnaire repository.</param>
        /// <param name="questionnaireService">The questionnaire service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public DepartmentQuestionnaireService(
            IDepartmentQuestionnaireRepository repository,
            IQuestionnaireService questionnaireService,
            IMapper mapper,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "DepartmentQuestionnaireId", tokenParser)
        {
            _repository = repository;
            _questionnaireService = questionnaireService;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new department questionnaire or updates an existing one.
        /// </summary>
        /// <param name="dto">The department questionnaire DTO to insert or update.</param>
        /// <returns>The upserted department questionnaire DTO.</returns>
        public override async Task<DepartmentQuestionnaireDto> UpsertAsync(DepartmentQuestionnaireDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate that the questionnaire has SourceQuestionnaireId (only derived questionnaires can be assigned to a department)
            if (dto.QuestionnaireId != Guid.Empty)
            {
                var questionnaire = await _questionnaireService.GetByIdAsync(dto.QuestionnaireId).ConfigureAwait(false);
                if (questionnaire == null)
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID));
                }
                //if (!questionnaire.SourceQuestionnaireId.HasValue || questionnaire.SourceQuestionnaireId.Value == Guid.Empty)
                //{
                //    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                //        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.QUESTIONNAIRE_MUST_HAVE_SOURCE_QUESTIONNAIRE_ID));
                //}
            }

            // Validate for duplicate: DepartmentID + QuestionnaireId combination must be unique
            if (dto.QuestionnaireId != Guid.Empty)
            {
                await ValidateDuplicateAsync(
                    propertyName: "QuestionnaireId",
                    value: dto.QuestionnaireId,
                    dto: dto,
                    isDepartmentScoped: true,
                    additionalFilter: e => e.QuestionnaireId == dto.QuestionnaireId,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_DEPARTMENT_QUESTIONNAIRE);
            }

            return await base.UpsertAsync(dto);
        }

        /// <inheritdoc />
        public async Task<PagedList<QuestionnaireDto>> GetQuestionnairesByAdminAsync(PageParam? pagination = null)
        {
            var allDepartmentQuestionnaires = await GetAllAsync(null, null, null).ConfigureAwait(false);
            var assignedQuestionnaireIds = allDepartmentQuestionnaires
                .Select(dq => dq.QuestionnaireId)
                .Distinct()
                .ToHashSet();

            return await _questionnaireService.GetAllPagedAsync(
                q => !assignedQuestionnaireIds.Contains(q.QuestionnaireId),
                pagination).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DepartmentQuestionnaireResponseDto>> GetQuestionnairesByDepartmentAsync(PageParam? pagination = null)
        {
            var departmentId = _tokenParser?.GetDepartmentID();
            if (!departmentId.HasValue)
                return Array.Empty<DepartmentQuestionnaireResponseDto>();

            // Use Include to join with Questionnaire in a single query
            var includes = new Expression<Func<DepartmentQuestionnaire, object>>[]
            {
                dq => dq.Questionnaire
            };

            var departmentQuestionnaires = (await GetAllPagedAsync(
                dq => dq.DepartmentID == departmentId.Value,
                pagination,
                includes).ConfigureAwait(false)).Data.ToList();

            // Ensure Activity Forms are sorted by Questionnaire Title (Form Name) A-Z
            var ordered = departmentQuestionnaires
                .Where(dq => dq.Questionnaire != null)
                .OrderBy(dq => dq.Questionnaire!.Title)
                .ToList();

            // Map directly from entities with included Questionnaire navigation property using constructor
            return ordered.Select(dq => new DepartmentQuestionnaireResponseDto(
                dq.Questionnaire!.QuestionnaireId,
                dq.Questionnaire.Title,
                dq.Questionnaire.Description,
                dq.Questionnaire.IsActive,
                dq.Questionnaire.QuestionnaireType,
                dq.Questionnaire.SourceQuestionnaireId,
                dq.DepartmentQuestionnaireId));
        }

        public override Task<PagedList<DepartmentQuestionnaireDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentQuestionnaire, bool>>? filter = null,
            Expression<Func<DepartmentQuestionnaire, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            var effectiveIncludes = includes ?? new Expression<Func<DepartmentQuestionnaire, object>>[] { x => x.Questionnaire };
            Expression<Func<DepartmentQuestionnaire, bool>> defaultFilter = filter ?? (x => true);

            return SearchPagedAsync(
                searchTerm,
                defaultFilter,
                term =>
                {
                    var lowered = term.Trim().ToLower();
                    return x =>
                        x.Questionnaire != null &&
                        (
                            (x.Questionnaire.Title != null && x.Questionnaire.Title.ToLower().Contains(lowered)) ||
                            (x.Questionnaire.Description != null && x.Questionnaire.Description.ToLower().Contains(lowered))
                        );
                },
                pageParam,
                effectiveIncludes);
        }

        public async Task<PagedList<DepartmentQuestionnaireResponseDto>> SearchQuestionnairesByDepartmentAsync(string? searchTerm, PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            
            // Remove SortBy to avoid repository error since Title is not a direct property of DepartmentQuestionnaire
            var originalSortBy = pagination.SortBy;
            pagination.SortBy = null;

            var departmentId = _tokenParser?.GetDepartmentID();
            if (!departmentId.HasValue)
                return new PagedList<DepartmentQuestionnaireResponseDto>(new List<DepartmentQuestionnaireResponseDto>(), pagination, 0);

            var includes = new Expression<Func<DepartmentQuestionnaire, object>>[]
            {
                dq => dq.Questionnaire
            };

            var paged = await SearchAsync(
                searchTerm,
                pagination,
                dq => dq.DepartmentID == departmentId.Value,
                includes).ConfigureAwait(false);

            var mapped = paged.Data
                .Where(dq => dq.Questionnaire != null)
                .Select(dq => new DepartmentQuestionnaireResponseDto(
                    dq.Questionnaire!.QuestionnaireId,
                    dq.Questionnaire.Title,
                    dq.Questionnaire.Description,
                    dq.Questionnaire.IsActive,
                    dq.Questionnaire.QuestionnaireType,
                    dq.Questionnaire.SourceQuestionnaireId,
                    dq.DepartmentQuestionnaireId))
                .ToList();

            // Apply sorting in the service layer since the repository can't handle nested properties
            if (!string.IsNullOrWhiteSpace(originalSortBy) && originalSortBy == nameof(DepartmentQuestionnaireResponseDto.Title))
            {
                var sortDirection = pagination.SortDirection?.ToLower() ?? "asc";
                mapped = sortDirection == "desc" 
                    ? mapped.OrderByDescending(x => x.Title).ToList()
                    : mapped.OrderBy(x => x.Title).ToList();
            }

            return new PagedList<DepartmentQuestionnaireResponseDto>(mapped, pagination, paged.TotalRecords);
        }

        private async Task<PagedList<QuestionnaireDto>> SearchPagedUnassignedQuestionnairesAsync(string? searchTerm, PageParam pagination)
        {
            var allDepartmentQuestionnaires = await GetAllAsync(null, null, null).ConfigureAwait(false);
            var assignedQuestionnaireIds = allDepartmentQuestionnaires
                .Select(dq => dq.QuestionnaireId)
                .Distinct()
                .ToHashSet();

            return await _questionnaireService.SearchPagedAsync(
                searchTerm,
                q => !assignedQuestionnaireIds.Contains(q.QuestionnaireId),
                term =>
                {
                    var lowered = term.Trim().ToLower();
                    return q =>
                        (q.Title != null && q.Title.ToLower().Contains(lowered)) ||
                        (q.Description != null && q.Description.ToLower().Contains(lowered));
                },
                pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Searches questionnaires by admin with optional search term and pagination.
        /// </summary>
        /// <param name="searchTerm">The search term to filter questionnaires.</param>
        /// <param name="pagination">The pagination parameters.</param>
        /// <returns>A paginated list of questionnaires matching the search criteria.</returns>
        public async Task<PagedList<QuestionnaireDto>> SearchQuestionnairesByAdminAsync(string? searchTerm, PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(QuestionnaireDto.Title);
            }

            var allDepartmentQuestionnaires = await GetAllAsync(null, null, null).ConfigureAwait(false);
            var assignedQuestionnaireIds = allDepartmentQuestionnaires
                .Select(dq => dq.QuestionnaireId)
                .Distinct()
                .ToHashSet();

            return await _questionnaireService.SearchPagedAsync(
                searchTerm,
                q => !assignedQuestionnaireIds.Contains(q.QuestionnaireId),
                term =>
                {
                    var lowered = term.Trim().ToLower();
                    return q =>
                        (q.Title != null && q.Title.ToLower().Contains(lowered)) ||
                        (q.Description != null && q.Description.ToLower().Contains(lowered));
                },
                pagination).ConfigureAwait(false);
        }
    }
}
