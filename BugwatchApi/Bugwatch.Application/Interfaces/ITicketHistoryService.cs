using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketHistoryService
{
    Task AddHistoryToTicketAsync(BasicTicket newTicket, string authId);
    TicketHistory CreateNewInstance(Guid ticketId);
}