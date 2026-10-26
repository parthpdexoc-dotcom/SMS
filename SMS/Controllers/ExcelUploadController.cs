using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers;

/// <summary>
/// API for Excel upload, export, and folder read operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExcelUploadController : ControllerBase
{
    private readonly IExcelService _excelService;
    private readonly IExcelFolderPathProvider _pathProvider;

    public ExcelUploadController(IExcelService excelService, IExcelFolderPathProvider pathProvider)
    {
        _excelService = excelService;
        _pathProvider = pathProvider;
    }

    /// <summary>
    /// Reads all .xlsx files from the configured Excel folder and returns rows as a flat array (first sheet per file).
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ProducesResponseType(typeof(List<Dictionary<string, string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<Dictionary<string, string>>> Upload()
    {
        var folderPath = _pathProvider.GetPath();
        var result = _excelService.ReadFromFolder(folderPath);

        if (result.ErrorMessage != null)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Rows);
    }

}
