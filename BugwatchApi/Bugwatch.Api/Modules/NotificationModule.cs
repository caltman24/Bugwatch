namespace Bugwatch.Api.Modules;

public static class NotificationModule
{
    // TODO: Notification routes
    public static IEndpointRouteBuilder UseNotificationModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/notifications");
        return app;
    }
}