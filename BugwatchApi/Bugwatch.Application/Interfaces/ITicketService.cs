using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketService
{
    Task UpdateWithHistoryAsync(Guid ticketId, BasicTicket ticket, string authId);
    Task CreateWithHistoryAsync(BasicTicket newTicket, string authId);
}