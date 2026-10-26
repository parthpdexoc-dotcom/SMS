using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing CancelledPostPosition operations using the generic service pattern.
    /// </summary>
    public class CancelledPostPositionService : GenericService<CancelledPostPositionDto, CancelledPostPosition>, ICancelledPostPositionService
    {
        private readonly IPostService _postService;
        private readonly IPostPositionService _postPositionService;
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IResponseService _responseService;
        private readonly ITokenParser? _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledPostPositionService"/> class.
        /// </summary>
        /// <param name="repository">The cancelled post position repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="postService">The post service for FK validation.</param>
        /// <param name="postPositionService">The post position service for FK validation.</param>
        /// <param name="departmentEquipmentService">The department equipment service for FK validation.</param>
        /// <param name="responseService">The response service for FK validation (questionnaire response).</param>
        /// <param name="tokenParser">The token parser for authentication (EmployeeID is set from token via IEmployeeScoped).</param>
        public CancelledPostPositionService(
            ICancelledPostPositionRepository repository,
            IMapper mapper,
            IPostService postService,
            IPostPositionService postPositionService,
            IDepartmentEquipmentService departmentEquipmentService,
            IResponseService responseService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "CancelledPostPositionID", tokenParser)
        {
            _postService = postService;
            _postPositionService = postPositionService;
            _departmentEquipmentService = departmentEquipmentService;
            _responseService = responseService;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new cancelled post position record or updates an existing one.
        /// </summary>
        /// <param name="dto">The cancelled post position DTO to insert or update.</param>
        /// <returns>The upserted cancelled post position DTO.</returns>
        public override async Task<CancelledPostPositionDto> UpsertAsync(CancelledPostPositionDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (dto.CancelledPostPositionID.HasValue && dto.CancelledPostPositionID.Value != Guid.Empty)
            {
                var existing = await GetByIdAsync(dto.CancelledPostPositionID.Value);
                if (existing != null && _tokenParser != null)
                {
                    var currentEmployeeId = _tokenParser.GetEmployeeID();
                    if (currentEmployeeId.HasValue && existing.EmployeeID != currentEmployeeId.Value)
                    {
                        var message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CANCELLED_JOB_POSITION_UPDATE_OWN_RECORD_ONLY)
                            ?? ModelValidationErrorCodes.CANCELLED_JOB_POSITION_UPDATE_OWN_RECORD_ONLY;
                        throw new UnauthorizedAccessException(message);
                    }
                }
            }

            // When both set: validate PostPosition exists and belongs to Post (validates both FKs in one go)
            if (dto.PostPositionID.HasValue && dto.PostID.HasValue)
            {
                var postPosition = await _postPositionService.GetByIdAsync(dto.PostPositionID.Value);
                if (postPosition == null || postPosition.PostID != dto.PostID.Value)
                {
                    var message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_JOB_AND_JOB_POSITION)
                        ?? ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_JOB_AND_JOB_POSITION;
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        message);
                }
            }
            else
            {
                if (dto.PostPositionID.HasValue)
                    await ValidateForeignKeyAsync<IPostPositionService, PostPositionDto>(
                        _postPositionService, dto.PostPositionID.Value, "PostPosition");
                if (dto.PostID.HasValue)
                    await ValidateForeignKeyAsync<IPostService, PostDto>(
                        _postService, dto.PostID.Value, "Post");
            }

            if (dto.PostID.HasValue && dto.PostPositionID.HasValue)
            {
                await ValidateCompositeKeyDuplicateAsync(
                    dto: dto,
                    filterExpression: e => e.PostID == dto.PostID && e.PostPositionID == dto.PostPositionID,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_CANCELLED_JOB_POSITION,
                    isEmployeeScoped: true);
            }

            if (dto.DepartmentEquipmentID.HasValue)
            {
                await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(
                    _departmentEquipmentService, dto.DepartmentEquipmentID.Value, "DepartmentEquipment");
            }

            if (dto.QuestionaireResponseID.HasValue)
            {
                await ValidateForeignKeyAsync<IResponseService, ResponseDto>(
                    _responseService, dto.QuestionaireResponseID.Value, "QuestionaireResponse");
            }

            return await base.UpsertAsync(dto);
        }
    }
}
