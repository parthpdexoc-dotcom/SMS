using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing Response operations using the generic service pattern.
    /// </summary>
    public class ResponseService : GenericService<ResponseDto, Response>, IResponseService
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IMapper _mapper;
        private readonly ITokenParser? _tokenParser;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseService"/> class.
        /// </summary>
        /// <param name="repository">The response repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        /// <param name="employeeService">The employee service for fetching employee data.</param>
        public ResponseService(
            IResponseRepository repository,
            IMapper mapper,
            IEmployeeService employeeService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "ResponseId", tokenParser)
        {
            _responseRepository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Inserts a new response or updates an existing one.
        /// </summary>
        /// <param name="dto">The response DTO to insert or update.</param>
        /// <returns>The upserted response DTO.</returns>
        public override async Task<ResponseDto> UpsertAsync(ResponseDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Get EmployeeID from token and fetch employee email
            var employeeId = _tokenParser?.GetEmployeeID();
            if (employeeId.HasValue && employeeId.Value != Guid.Empty)
            {
                var employee = await _employeeService.GetByIdAsync(employeeId.Value);
                if (employee != null && !string.IsNullOrEmpty(employee.PrimaryEmail))
                {
                    dto.SubmitterEmail = employee.PrimaryEmail;
                }
            }

            // Validate for duplicate: QuestionnaireId + PostPositionId + CreatedUserID (SubmittedBy) combination must be unique
            //if (dto.CreatedUserID.HasValue && dto.CreatedUserID.Value != Guid.Empty)
            //{
            //    await ValidateDuplicateAsync(
            //        propertyName: "CreatedUserID",
            //        value: dto.CreatedUserID.Value,
            //        dto: dto,
            //        isDepartmentScoped: false,
            //        additionalFilter: e => 
            //            e.QuestionnaireId == dto.QuestionnaireId && 
            //            e.PostPositionId == dto.PostPositionId,
            //        errorMessage: ModelValidationErrorCodes.DUPLICATE_RESPONSE);
            //}

            // Validate for duplicate: QuestionnaireId + PostPositionId + SubmitterEmail combination must be unique
            if (!string.IsNullOrEmpty(dto.SubmitterEmail))
            {
                await ValidateDuplicateAsync(
                    propertyName: "SubmitterEmail",
                    value: dto.SubmitterEmail,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => 
                        e.QuestionnaireId == dto.QuestionnaireId && 
                        e.PostPositionId == dto.PostPositionId,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_RESPONSE_SUBMITTER_EMAIL);
            }

            return await base.UpsertAsync(dto);
        }
    }
}
