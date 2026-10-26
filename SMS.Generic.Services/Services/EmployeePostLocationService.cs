using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    public class EmployeePostLocationService : GenericService<EmployeePostLocationDto, EmployeePostLocation>, IEmployeePostLocationService
    {
        private readonly IEmployeePostLocationRepository _employeePostLocationRepository;
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly ITokenParser _tokenParser;

        public EmployeePostLocationService(
            IEmployeePostLocationRepository repository,
            IPostPositionRepository postPositionRepository,
            IMapper mapper,
            ITokenParser tokenParser)
            : base(repository, mapper, nameof(EmployeePostLocation.EmployeePostLocationID), tokenParser)
        {
            _employeePostLocationRepository = repository;
            _postPositionRepository = postPositionRepository;
            _tokenParser = tokenParser;
        }

        /// <inheritdoc />
        public Task<List<EmployeePostLocationDto>> GetCoordinatesByEmployeeIdAsync(
            Guid employeeId,
            Guid postPositionId,
            CancellationToken cancellationToken = default)
        {
            if (employeeId == Guid.Empty || postPositionId == Guid.Empty)
                return Task.FromResult(new List<EmployeePostLocationDto>());

            return _employeePostLocationRepository.GetCoordinatesByEmployeeIdAsync(employeeId, postPositionId, cancellationToken);
        }

        public async Task<EmployeePostLocationDto> SavePostLocationAsync(EmployeePostLocationDto request)
        {
            if (_tokenParser.GetEmployeeID() is not Guid employeeId || employeeId == Guid.Empty)
                throw new WebApiException(HttpStatusCode.Unauthorized, ((int)HttpStatusCode.Unauthorized).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_EMPLOYEE_ID_NOT_FOUND_IN_TOKEN));

            var postPosition = await _postPositionRepository.GetByIdAsync(
                request.PostPositionID,
                nameof(PostPosition.PostPositionID),
                position => position.Post!).ConfigureAwait(false)
                ?? throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, request.PostPositionID));

            if (postPosition.IsPlaceholder
                || postPosition.PostPositionStatus != PostPositionStatus.Assigned
                || postPosition.EmployeeID != employeeId)
                throw new WebApiException(HttpStatusCode.Forbidden, ((int)HttpStatusCode.Forbidden).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_LOCATION_NOT_ASSIGNED_TO_YOU));

            var startTime = postPosition.StartTime ?? postPosition.Post?.PostStartTime;
            var endTime = postPosition.EndTime ?? postPosition.Post?.PostEndTime;
            if (postPosition.DateOfWork is not DateOnly workDate || startTime is not TimeOnly start || endTime is not TimeOnly end)
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_LOCATION_SCHEDULE_MISSING));

            var shiftStart = workDate.ToDateTime(start);
            var shiftEnd = workDate.ToDateTime(end);
            if (shiftEnd <= shiftStart)
                shiftEnd = shiftEnd.AddDays(1);

            var now = GetCurrentDepartmentDateTime();
            if (now < shiftStart.AddMinutes(-30))
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_LOCATION_TOO_EARLY));
            if (now > shiftEnd.AddMinutes(30))
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_LOCATION_SHIFT_ENDED));

            return await UpsertAsync(new EmployeePostLocationDto
            {
                PostPositionID = postPosition.PostPositionID,
                EmployeeID = employeeId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CapturedAt = DateTime.UtcNow
            }).ConfigureAwait(false);
        }
    }
}
