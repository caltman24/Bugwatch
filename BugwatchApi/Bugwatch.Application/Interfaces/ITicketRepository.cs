using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketRepository
{
    Task<IQueryable<BasicTicket>> GetAllAsync(string authId);

    Task<IQueryable<BasicTicket>> GetProjectTicketsAsync(Guid projectId);

    Task<Ticket?> GetByIdAsync(Guid ticketId);
    Task InsertAsync(BasicTicket newTicket, TicketHistory? ticketHistory, string authId);
    Task UpdateAsync(Guid ticketId, BasicTicket updatedTicket);
    Task UpdateStatusAsync(Guid ticketId, string status, TicketHistory? ticketHistory, string authId);
    Task UpdateDeveloperAsync(Guid ticketId, Guid developerId);
    Task DeleteAsync(Guid ticketId);
}