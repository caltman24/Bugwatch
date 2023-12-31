﻿using Bugwatch.Api.Helpers;
using Bugwatch.Infrastructure.Repositories;

namespace Bugwatch.Api.Filters;

public static class TeamMemberRoleFilterExtensions
{
    public static TBuilder WithMemberRole<TBuilder>(this TBuilder builder, params string[] roles)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (ctx, next) =>
        {
            var teamMemberRepository = ctx.HttpContext.RequestServices.GetRequiredService<ITeamMemberRepository>();

            var authId = ContextHelper.GetNameIdentifier(ctx.HttpContext)!;

            var memberRole = await teamMemberRepository.GetRoleAsync(authId);

            if (memberRole == null)
                return Results.Problem(statusCode: 401, title: "Unauthorized",
                    detail: "Insufficient permission to access. No role");

            if (!roles.Any(role => role.ToLower().Equals(memberRole.ToLower())))
                return Results.Problem(statusCode: 401, title: "Unauthorized",
                    detail: $"Insufficient permission to access. Role: {memberRole}");

            ctx.HttpContext.Items.Add("role", memberRole);
            return await next(ctx);
        });

        return builder;
    }
}