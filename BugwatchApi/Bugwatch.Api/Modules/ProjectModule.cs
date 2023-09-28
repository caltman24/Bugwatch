using Bugwatch.Api.Filters;
using Bugwatch.Api.Helpers;
using Bugwatch.Application;
using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bugwatch.Api.Modules;

public static class ProjectModule
{
    public static IEndpointRouteBuilder UseProjectModule(this IEndpointRouteBuilder app)
    {
        var projectGroup = app.MapGroup("/projects")
            .RequireAuthorization()
            .AddEndpointFilter<ProjectValidationFilter>()
            .WithTags("Project")
            .WithOpenApi();

        projectGroup.MapGet("/{projectId:guid}", async Task<Results<Ok<Project>, NotFound>> (
                IProjectRepository projectRepository,
                [FromRoute] Guid projectId) =>
            {
                var project = await projectRepository.GetByIdAsync(projectId);

                return project is null ? TypedResults.NotFound() : TypedResults.Ok(project);
            }).WithName("GetProjectById")
            .WithDescription("Get project details and associated tickets");

        projectGroup.MapGet("/myprojects", async (
            HttpContext ctx,
            ProjectRepositoryResolver resolver
        ) =>
        {
            var role = ContextHelper.GetMemberRole(ctx);
            var authId = ContextHelper.GetNameIdentifier(ctx)!;

            var projectRepository = resolver(role); // Developer Or PM ProjectRepository

            var projects = await projectRepository.GetProjectsAsync(authId);

            return TypedResults.Ok(projects);
        }).WithMemberRole(UserRoles.Developer, UserRoles.ProjectManager);


        projectGroup.MapPut("/{projectId:guid}", async (
            IProjectRepository projectRepository,
            [FromRoute] Guid projectId,
            [FromBody] UpsertProjectRequest upsertProjectRequest) =>
        {
            await projectRepository.UpdateAsync(projectId, new BasicProject
            {
                Id = projectId,
                Description = upsertProjectRequest.Description,
                Title = upsertProjectRequest.Title,
                Priority = upsertProjectRequest.Priority,
                Status = upsertProjectRequest.Status,
                ProjectManagerId = upsertProjectRequest.ProjectManagerId,
                Deadline = upsertProjectRequest.Deadline
            });

            return TypedResults.NoContent();
        }).WithMemberRole(UserRoles.ProjectManager, UserRoles.Admin);


        return app;
    }
}