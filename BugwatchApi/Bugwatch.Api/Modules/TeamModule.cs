using Bugwatch.Api.Filters;
using Bugwatch.Api.Helpers;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bugwatch.Api.Modules;

public static class TeamModule
{
    public static IEndpointRouteBuilder UseTeamModule(this IEndpointRouteBuilder app)
    {
        var teamGroup = app.MapGroup("/team")
            .RequireAuthorization()
            .WithTags("Team");

        teamGroup.MapGet("/", async Task<Results<Ok<GetTeamResponse>, NotFound>> (
            ITeamRepository teamRepository,
            HttpContext ctx) =>
        {
            var authId = ContextHelper.GetNameIdentifier(ctx)!;
            var team = await teamRepository.GetByAuthIdAsync(authId);

            if (team == null) return TypedResults.NotFound();

            // We map from the contact to the domain model because there may be things we want to add or remove
            var response = new GetTeamResponse(
                team.Id,
                team.CreatorId,
                team.CreatedAt,
                team.UpdatedAt);

            return TypedResults.Ok(response);
        }).WithName("GetTeamById");

        teamGroup.MapPost("/", async Task<Results<CreatedAtRoute<Team>, ProblemHttpResult>> (
            HttpContext ctx,
            ITeamRepository teamRepository,
            NewTeamRequest newTeamRequest) =>
        {
            var authId = ContextHelper.GetNameIdentifier(ctx)!;

            var newTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = newTeamRequest.Name,
                CreatedAt = DateTime.UtcNow
            };

            var success = await teamRepository.InsertAsync(newTeam, authId);

            if (!success)
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status409Conflict,
                    detail: "Team member is already apart of a team", title: "Team member exists");
            }

            return TypedResults.CreatedAtRoute(newTeam, "GetTeamById", new { authId });
        });

        teamGroup.MapGet("/projects", async (
            IProjectRepository projectRepository,
            HttpContext ctx) =>
        {
            var authId = ContextHelper.GetNameIdentifier(ctx)!;
            var projects = await projectRepository.GetAllAsync(authId);

            // We dont map from domain model to a contract because we already have just what we need
            return TypedResults.Ok(projects);
        }).AddEndpointFilter<ProjectValidationFilter>();


        return app;
    }
}