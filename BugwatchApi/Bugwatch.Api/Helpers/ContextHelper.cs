namespace Bugwatch.Api.Helpers;

public static class ContextHelper
{
    /// <summary>
    ///     Looks in the HttpContext Items for the role provided by the MemberRoleFilter
    /// </summary>
    /// <param name="ctx">HttpContext</param>
    /// <returns>Role assigned to team member</returns>
    /// <exception cref="NullReferenceException">Thrown when no role exists</exception>
    public static string GetMemberRole(HttpContext ctx)
    {
        ctx.Items.TryGetValue("role", out var role);

        if (role == null) throw new NullReferenceException("No member role found");

        return (string)role;
    }
}