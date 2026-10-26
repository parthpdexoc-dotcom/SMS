using Microsoft.AspNetCore.Mvc;

namespace SMS.Filters;

/// <summary>
/// Authorizes access if the user satisfies any one of the given policies (OR condition).
/// Use like [Authorize(Policy = "X")] but pass multiple policies: [AuthorizeAny("Policy1", "Policy2")].
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeAnyAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Require any one of the given policies to pass (OR condition).
    /// </summary>
    /// <param name="policies">Policy names, e.g. nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly).</param>
    public AuthorizeAnyAttribute(params string[] policies) : base(typeof(AuthorizeAnyFilter))
    {
        Arguments = new object[] { policies };
    }
}
