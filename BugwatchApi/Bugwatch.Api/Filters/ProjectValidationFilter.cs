using Bugwatch.Application.Validators;
using Bugwatch.Contracts;

namespace Bugwatch.Api.Filters;

public class ProjectValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var method = context.HttpContext.Request.Method;

        if (method != HttpMethods.Put && method != HttpMethods.Post) return await next(context);

        var model = context.Arguments.FirstOrDefault(a => a is ProjectRequest) as ProjectRequest;

        var validStatus = StatusValidator.ValidateProject(model?.Status);
        var validPriority = PriorityValidator.Validate(model?.Priority);

        if (!validStatus) return Results.Problem(statusCode: 400, title: "Invalid status");
        if (!validPriority) return Results.Problem(statusCode: 400, title: "Invalid priority");

        return await next(context);
    }
}