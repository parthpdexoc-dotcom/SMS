using Microsoft.AspNetCore.Http;
using SMS.Generic.Dtos.Dtos;
using System.Security.Claims;

namespace SMS.Generic.Services.Helpers;

public sealed class JobHttpContextScope : IDisposable
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext? _previous;

    public JobHttpContextScope(
        IHttpContextAccessor httpContextAccessor,
        Guid departmentId,
        Guid employeeId,
        string timeZoneId,
        DepartmentPolicyDto? departmentPolicy)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        _previous = _httpContextAccessor.HttpContext;

        var claims = new List<Claim>
        {
            // TokenParser reads these claim types in your existing codebase.
            new("DepartmentID", departmentId.ToString()),
            new("EmployeeID", employeeId.ToString()),
            new("TimeZoneId", string.IsNullOrWhiteSpace(timeZoneId) ? "Eastern Standard Time" : timeZoneId),
        };

        if (departmentPolicy != null)
        {
            // Store as JSON string claim (TokenParser expects serialized policy configuration).
            claims.Add(new("DepartmentPolicyConfiguration", System.Text.Json.JsonSerializer.Serialize(departmentPolicy)));
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "Job");
        var principal = new ClaimsPrincipal(identity);

        _httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = principal
        };
    }

    public void Dispose()
    {
        _httpContextAccessor.HttpContext = _previous;
    }
}

