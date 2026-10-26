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
    /// Service for managing Option operations using the generic service pattern.
    /// </summary>
    public class OptionService : GenericService<OptionDto, Option>, IOptionService
    {
        private readonly IOptionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IQuestionService _questionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionService"/> class.
        /// </summary>
        /// <param name="repository">The Option repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="questionService">The question service for foreign key validation.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public OptionService(
            IOptionRepository repository,
            IMapper mapper,
            IQuestionService questionService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "OptionId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _questionService = questionService;
        }

        /// <summary>
        /// Inserts a new Option or updates an existing one.
        /// </summary>
        /// <param name="dto">The Option DTO to insert or update.</param>
        /// <returns>The upserted Option DTO.</returns>
        public override async Task<OptionDto> UpsertAsync(OptionDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate QuestionId foreign key exists
            if (dto.QuestionId.HasValue && dto.QuestionId.Value != Guid.Empty)
            {
                await ValidateForeignKeyAsync<IQuestionService, QuestionDto>(_questionService, dto.QuestionId.Value, "Question");
            }

            // Validate for duplicate DisplayOrder within the same Question
            if (dto.DisplayOrder > 0 && dto.QuestionId.HasValue && dto.QuestionId.Value != Guid.Empty)
            {
                await ValidateDuplicateAsync(
                    propertyName: "DisplayOrder",
                    value: dto.DisplayOrder,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => e.QuestionId == dto.QuestionId.Value,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_OPTION_DISPLAY_ORDER);
            }

            return await base.UpsertAsync(dto);
        }
    }
}
