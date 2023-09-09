using Bugwatch.Application.Validators;
using Bugwatch.Contracts;

namespace Bugwatch.Api.Filters;

public sealed class TicketValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var method = context.HttpContext.Request.Method;

        if (method != HttpMethods.Put && method != HttpMethods.Post) return await next(context);
        
        var model = context.Arguments.FirstOrDefault(a => a is TicketRequest) as TicketRequest;
        
        var validStatus = StatusValidator.ValidateTicket(model?.Status);
        var validPriority = PriorityValidator.Validate(model?.Priority);
        var validType = TicketTypeValidator.Validate(model?.Type);
        
        if (!validStatus) return Results.Problem(statusCode: 400, title: "Invalid status");
        if (!validPriority) return Results.Problem(statusCode: 400, title: "Invalid priority");
        if (!validType) return Results.Problem(statusCode: 400, title: "Invalid ticket type");
        
        return await next(context);
    }
}