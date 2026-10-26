using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for manual payroll export generation.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollExportController : ControllerBase
    {
        private readonly IPayrollProcessingService _service;
        private readonly IPayrollNotificationService _notificationService;
        private readonly IDepartmentService _departmentService;

        public PayrollExportController(
            IPayrollProcessingService service,
            IPayrollNotificationService notificationService,
            IDepartmentService departmentService)
        {
            _service = service;
            _notificationService = notificationService;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Processes payroll data for the specified date range and department.
        /// </summary>
        [HttpPost("Generate")]
        [ProducesResponseType(typeof(PayrollProcessingResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        public async Task<ActionResult<PayrollProcessingResultDto>> ProcessPayrollDataAsync(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate,
            [FromQuery] Guid? departmentId,
            [FromQuery] string? customFileName = null)
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be greater than end date.");

            var result = await _service.ProcessPayrollDataAsync(startDate, endDate, customFileName, departmentId);

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                var department = await _departmentService.GetByIdAsync(departmentId.Value);
                var departmentName = string.IsNullOrWhiteSpace(department?.DepartmentName)
                    ? departmentId.Value.ToString()
                    : department.DepartmentName.Trim();

                await _notificationService.NotifyAsync(
                    result,
                    departmentName,
                    result.IsSuccess ? PayrollRunStatus.Success : PayrollRunStatus.Failed);
            }

            return Ok(result);
        }
    }
}
