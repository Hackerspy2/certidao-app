#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace WebApp.Filter;

public class PermissionRequired : IAuthorizationRequirement
{
    public PermissionRequired(string[] _permissions)
    {
        Permissions = _permissions;
    }

    public string[] Permissions { get; set; }
}

public class ClaimRequirementFilter : IAuthorizationFilter
{
    private readonly PermissionRequired rules;
    public ClaimRequirementFilter(PermissionRequired _permissions)
    {
        rules = _permissions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Claims.Any())
        {
            context.Result = new RedirectResult("~/entrar");
            return;
        }

        var isSuporte = bool.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "IsSuporte")?.Value ?? string.Empty);
        if (!isSuporte)
        {
            if (rules.Permissions.Length == 0) context.Result = new RedirectResult("/nao-autorizado");
            var hasClaim = context.HttpContext.User.Claims.Any(c => rules.Permissions.Any(_ => _ == c.Type));
            if (hasClaim) return;
            context.Result = new RedirectResult("/nao-autorizado");
        }
    }
}

public class ClaimAutenticateFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Claims.Any()) return;
        context.Result = new RedirectResult("~/entrar");
    }
}

public class RequiredPermissionAttribute : TypeFilterAttribute
{
    public RequiredPermissionAttribute(params string[] permissions) : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] { new PermissionRequired(permissions) };
    }
}

public class AllPermissionAttribute : TypeFilterAttribute
{
    public AllPermissionAttribute() : base(typeof(ClaimAutenticateFilter))
    {
    }
}