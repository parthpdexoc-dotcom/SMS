using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SMS.Filters;

/// <summary>
/// Authorization filter that allows access if the user satisfies any one of the given policies (OR condition).
/// </summary>
public class AuthorizeAnyFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthorizationService _authorizationService;
    private readonly string[] _policies;

    public AuthorizeAnyFilter(IAuthorizationService authorizationService, string[] policies)
    {
        _authorizationService = authorizationService;
        _policies = policies ?? Array.Empty<string>();
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_policies.Length == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        var user = context.HttpContext.User;
        foreach (var policy in _policies)
        {
            var result = await _authorizationService.AuthorizeAsync(user, policy);
            if (result.Succeeded)
                return;
        }

        context.Result = new ForbidResult();
    }
}
