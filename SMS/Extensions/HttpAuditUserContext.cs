using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using SMS.Generic.Common.Audit;
using SMS.Generic.Services.Interfaces;

namespace SMS.Infrastructure.Audit;

/// <summary>
/// Resolves audit user and correlation from the current HTTP request (JWT + optional correlation header).
/// </summary>
public sealed class HttpAuditUserContext : IAuditUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenParser _tokenParser;

    public HttpAuditUserContext(IHttpContextAccessor httpContextAccessor, ITokenParser tokenParser)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenParser = tokenParser;
    }

    /// <inheritdoc />
    public string? UserId =>
        _tokenParser.GetAdminID()?.ToString()
        ?? _tokenParser.GetEmployeeID()?.ToString()
        ?? _tokenParser.GetUsername();

    /// <inheritdoc />
    public Guid? DepartmentID => _tokenParser.GetDepartmentID();

    /// <inheritdoc />
    public string? CorrelationId =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault()
        ?? Activity.Current?.Id;
}
