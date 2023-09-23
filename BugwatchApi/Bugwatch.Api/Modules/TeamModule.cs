using Bugwatch.Api.Filters;
using Bugwatch.Api.Helpers;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            var authId = ContextHelper.GetIdentityName(ctx)!;
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

        teamGroup.MapPost("/", async (
            ITeamRepository teamRepository,
            NewTeamRequest newTeamRequest) =>
        {
            var newTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = newTeamRequest.Name,
                CreatorId = newTeamRequest.CreatorId,
                CreatedAt = DateTime.UtcNow
            };

            await teamRepository.InsertAsync(newTeam);

            return TypedResults.CreatedAtRoute(newTeam, "GetTeamById", new { authId = "" });
        });

        teamGroup.MapGet("/projects", async (
            IProjectRepository projectRepository,
            HttpContext ctx) =>
        {
            var authId = ContextHelper.GetIdentityName(ctx)!;
            var projects = await projectRepository.GetAllAsync(authId);

            // We dont map from domain model to a contract because we already have just what we need
            return TypedResults.Ok(projects);
        }).AddEndpointFilter<ProjectValidationFilter>();

        teamGroup.MapPut("/{teamId:Guid}", async (
            ITeamRepository teamRepository,
            Guid teamId,
            UpdateTeamRequest updateTeamRequest) =>
        {
            await teamRepository.UpdateAsync(teamId, updateTeamRequest.Name);

            return TypedResults.NoContent();
        });

        teamGroup.MapDelete("/{teamId:Guid}", async (
            ITeamRepository teamRepository,
            Guid teamId) =>
        {
            await teamRepository.DeleteAsync(teamId);

            return TypedResults.NoContent();
        });

        return app;
    }
}