using Bugwatch.Infrastructure.Repositories;

namespace Bugwatch.Api.Filters;

public static class UserFilterExtensions
{
    public static TBuilder WithExistingUser<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (ctx, next) =>
        {
            var teamMemberRepository = ctx.HttpContext.RequestServices.GetRequiredService<ITeamMemberRepository>();

            // FIXME: For testing purposes only
            var authId = ctx.GetArgument<string>(0);
            // var authId = ctx.HttpContext.User.Identity?.Name!;

            var userExists = await teamMemberRepository.UserExistsAsync(authId);

            if (!userExists)
                return Results.Problem(statusCode: 401, title: "Unauthorized",
                    detail: "No user exists");

            return await next(ctx);
        });

        return builder;
    }
}