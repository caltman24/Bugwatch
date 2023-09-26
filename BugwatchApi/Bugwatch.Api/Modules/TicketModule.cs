using System.Reflection.Metadata;
using Bugwatch.Api.Filters;
using Bugwatch.Api.Helpers;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Application.ValueObjects;
using Bugwatch.Contracts;
using Bugwatch.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bugwatch.Api.Modules;

public static class TicketModule
{
    public static IEndpointRouteBuilder UseTicketModule(this IEndpointRouteBuilder app)
    {
        app.MapTicketGroup().MapTicketCommentGroup();

        return app;
    }

    private static IEndpointRouteBuilder MapTicketGroup(this IEndpointRouteBuilder app)
    {
        var ticketGroup = app.MapGroup("/tickets")
            .WithTags("Tickets")
            .RequireAuthorization();

        ticketGroup.MapGet("/{ticketId:Guid}", async Task<Results<Ok<GetTicketRequest>, NotFound>> (
            Guid ticketId,
            ITicketRepository ticketRepository) =>
        {
            var ticket = await ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null) return TypedResults.NotFound();

            var historyRequests = new List<GetTicketHistoryRequest>();

            foreach (var history in ticket?.TicketHistory!)
                historyRequests.Add(new GetTicketHistoryRequest(
                    history.Id,
                    history.TicketId,
                    history.Message,
                    history.EventName,
                    history.OldValue,
                    history.NewValue,
                    new TeamMemberInfo(
                        history.TeamMember.Id,
                        history.TeamMember.Name,
                        history.TeamMember.Username,
                        history.TeamMember.Role)));

            return TypedResults.Ok(new GetTicketRequest(
                ticket.Id,
                ticket.SubmitterId,
                ticket.DeveloperId,
                ticket.ProjectId,
                ticket.Title,
                ticket.Description,
                ticket.Priority,
                ticket.Status,
                ticket.Type,
                ticket.CreatedAt,
                ticket.UpdatedAt,
                historyRequests.AsEnumerable()));
        }).WithName("GetTicketById");

        ticketGroup.MapPost("/", async (
                HttpContext ctx,
                [FromBody] NewTicketRequest newTicketRequest,
                ITeamMemberRepository teamMemberRepository,
                ITicketService ticketService) =>
            {
                var authId = ContextHelper.GetIdentityName(ctx)!;
                var memberInfo = await teamMemberRepository.GetInfoAsync(authId);
                
                var newTicket = new BasicTicket
                {
                    Id = Guid.NewGuid(),
                    SubmitterId = memberInfo.Id,
                    DeveloperId = newTicketRequest.DeveloperId,
                    ProjectId = newTicketRequest.ProjectId,
                    Title = newTicketRequest.Title,
                    Description = newTicketRequest.Description,
                    Priority = newTicketRequest.Priority,
                    Status = newTicketRequest.Status,
                    Type = newTicketRequest.Type,
                    CreatedAt = DateTime.UtcNow
                };

                await ticketService.CreateWithHistoryAsync(newTicket, authId);

                return TypedResults.CreatedAtRoute(newTicket, "GetTicketById", new { ticketId = newTicket.Id });
            }).AddEndpointFilter<TicketValidationFilter>()
            .WithMemberRole(UserRoles.Admin, UserRoles.ProjectManager, UserRoles.Submitter);

        ticketGroup.MapPut("/{ticketId:Guid}", async (
            HttpContext ctx,
            Guid ticketId,
            ITicketService ticketService,
            UpdateTicketRequest updateTicketRequest) =>
        {
            var updatedTicket = new BasicTicket
            {
                Id = ticketId,
                Priority = updateTicketRequest.Priority,
                Status = updateTicketRequest.Status,
                Title = updateTicketRequest.Title,
                Description = updateTicketRequest.Description,
                Type = updateTicketRequest.Type,
                DeveloperId = updateTicketRequest.DeveloperId
            };

            var authId = ContextHelper.GetIdentityName(ctx)!;
            await ticketService.UpdateWithHistoryAsync(ticketId, updatedTicket, authId);

            return TypedResults.NoContent();
        }).WithExistingUser().AddEndpointFilter<TicketValidationFilter>();

        ticketGroup.MapPut("/assign/{ticketId:Guid}", async Task<Results<BadRequest<string>, NoContent>> (
            [FromQuery] Guid developerId,
            Guid ticketId,
            ITeamMemberRepository teamMemberRepository,
            ITicketRepository ticketRepository) =>
        {
            var isDeveloper = await teamMemberRepository.ValidateRoleAsync(developerId, UserRoles.Developer);

            if (!isDeveloper) return TypedResults.BadRequest("Team member is not a developer");

            await ticketRepository.UpdateDeveloperAsync(ticketId, developerId);

            return TypedResults.NoContent();
        });

        // TODO: If role is project manager, check if the ticket is apart of their project
        ticketGroup.MapDelete("/{ticketId:Guid}", async (
            Guid ticketId,
            ITicketRepository ticketRepository) =>
        {
            await ticketRepository.DeleteAsync(ticketId);

            return TypedResults.NoContent();
        }).WithMemberRole(UserRoles.Admin, UserRoles.ProjectManager);

        return ticketGroup;
    }

    // TODO: ticket comment routes
    private static IEndpointRouteBuilder MapTicketCommentGroup(this IEndpointRouteBuilder app)
    {
        var commentGroup = app.MapGroup("/comments").WithTags("Comments");
        commentGroup.MapGet("/", () => TypedResults.Ok("Yayyy comment"));
        return app;
    }
}