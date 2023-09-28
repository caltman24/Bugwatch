using Bugwatch.Api.Filters;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Bugwatch.Api.Modules;

public static class AdminModule
{
    // TODO: admin routes
    public static IEndpointRouteBuilder UseAdminModule(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup("/admin")
            .WithTags("Admin")
            .WithMemberRole(UserRoles.Admin);

        var adminProjectGroup = adminGroup.MapGroup("/project");

        adminProjectGroup.MapPost("/", async (
            [FromBody] NewProjectRequest newProjectResponse,
            IProjectRepository projectRepository,
            [FromQuery] Guid teamId) =>
        {
            var newProject = new BasicProject
            {
                Id = Guid.NewGuid(),
                Title = newProjectResponse.Title,
                Description = newProjectResponse.Description,
                TeamId = teamId,
                CreatedAt = DateTime.UtcNow,
                Priority = newProjectResponse.Priority,
                Status = newProjectResponse.Status,
                ProjectManagerId = newProjectResponse.ProjectManagerId,
                Deadline = newProjectResponse.Deadline
            };

            await projectRepository.InsertAsync(newProject);

            return TypedResults.CreatedAtRoute(
                newProject,
                "GetProjectById",
                new { projectId = newProject.Id });
        });

        adminProjectGroup.MapPut("/assign/{projectId:guid}", async (
            Guid projectId,
            [FromQuery] Guid projectManagerId,
            IProjectRepository projectRepository) =>
        {
            await projectRepository.AssignProjectManagerAsync(projectId, projectManagerId);

            return TypedResults.NoContent();
        });

        adminProjectGroup.MapPut("/unassign/{projectId:guid}", async (
            Guid projectId,
            IProjectRepository projectRepository) =>
        {
            await projectRepository.UnassignProjectManagerAsync(projectId);

            return TypedResults.NoContent();
        });

        adminProjectGroup.MapDelete("/{projectId:Guid}", async (
            IProjectRepository projectRepository,
            [FromRoute] Guid projectId) =>
        {
            await projectRepository.DeleteAsync(projectId);

            return TypedResults.NoContent();
        });

        var adminTeamGroup = adminGroup.MapGroup("/team");
        
        // Team routes
        adminTeamGroup.MapDelete("/{teamId:Guid}", async (
            ITeamRepository teamRepository,
            Guid teamId) =>
        {
            await teamRepository.DeleteAsync(teamId);

            return TypedResults.NoContent();
        });
        
        adminTeamGroup.MapPut("/{teamId:Guid}", async (
            ITeamRepository teamRepository,
            Guid teamId,
            UpdateTeamRequest updateTeamRequest) =>
        {
            await teamRepository.UpdateAsync(teamId, updateTeamRequest.Name);

            return TypedResults.NoContent();
        });
        
        return app;
    }
}