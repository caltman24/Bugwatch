using Bugwatch.Api.Filters;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Factories;
using Bugwatch.Application.Interfaces;
using Bugwatch.Application.ValueObjects;
using Bugwatch.Contracts;
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
        var ticketGroup = app.MapGroup("/tickets");

        ticketGroup.MapGet("/{ticketId:Guid}", async Task<Results<Ok<GetTicketRequest>, NotFound>> (
            Guid ticketId,
            ITicketRepository ticketRepository) =>
        {
            var ticket = await ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null) return TypedResults.NotFound();

            var historyRequests = new List<GetTicketHistoryRequest>();

            foreach (var h in ticket?.TicketHistory!)
                historyRequests.Add(new GetTicketHistoryRequest(
                    h.Id,
                    h.TicketId,
                    h.Message,
                    h.EventName,
                    h.OldValue,
                    h.NewValue,
                    new TeamMemberInfo(
                        h.TeamMember.Id,
                        h.TeamMember.Name,
                        h.TeamMember.Username,
                        h.TeamMember.Role)));

            return TypedResults.Ok(new GetTicketRequest(
                ticket.SubmitterId,
                ticket.DeveloperId,
                ticket.ProjectId,
                ticket.Title,
                ticket.Description,
                ticket.Priority,
                ticket.Status,
                ticket.Type,
                historyRequests.AsEnumerable()));
        }).WithName("GetTicketById");

        ticketGroup.MapPost("/", async (
            [FromBody] NewTicketRequest newTicketRequest,
            [FromQuery] string authId,
            ITicketService ticketService,
            ITicketHistoryFactory ticketHistoryFactory) =>
        {
            var newTicket = new BasicTicket
            {
                Id = Guid.NewGuid(),
                SubmitterId = newTicketRequest.SubmitterId,
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
        }).WithMemberRole(UserRoles.Admin, UserRoles.ProjectManager, UserRoles.Submitter);

        // TODO: Finish update ticket
        ticketGroup.MapPut("/{ticketId:Guid}", async (
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
                Type = updateTicketRequest.Type,
                DeveloperId = updateTicketRequest.DeveloperId
            };
            
            // await ticketService.UpdateWithHistoryAsync(ticketId, updatedTicket, "");
        });

        // TODO: Update ticket status
        // TODO: Assign developer to ticket

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
        var commentGroup = app.MapGroup("/comments");
        commentGroup.MapGet("/", () => TypedResults.Ok("Yayyy comment"));
        return app;
    }
}