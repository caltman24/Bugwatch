using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketRepository
{
    Task<IQueryable<BasicTicket>> GetAllAsync(string authId);

    Task<IQueryable<BasicTicket>> GetProjectTicketsAsync(Guid projectId);

    // Task<IQueryable<BasicTicket>> GetDeveloperTicketsAsync(string authId);
    // Task<IQueryable<BasicTicket>> GetSubmitterTicketsAsync(string authId);
    Task<Ticket?> GetByIdAsync(Guid ticketId);
    Task InsertAsync(BasicTicket newTicket, TicketHistory? ticketHistory, string authId);
    Task UpdateAsync(Guid ticketId, BasicTicket updatedTicket, TicketHistory? ticketHistory);
    Task UpdateStatusAsync(Guid ticketId, string status, TicketHistory? ticketHistory);
    Task DeleteAsync(Guid ticketId);
}