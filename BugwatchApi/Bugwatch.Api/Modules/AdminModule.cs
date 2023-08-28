using Bugwatch.Api.Filters;
using Bugwatch.Application.Constants;

namespace Bugwatch.Api.Modules;

public static class AdminModule
{
    // TODO: admin routes
    public static IEndpointRouteBuilder UseAdminModule(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup("/admin").WithMemberRole(UserRoles.Admin);
        return app;
    }
}