using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketHistoryFactory
{
    TicketHistory CreateFromEvent(Guid ticketId, string eventName, string? oldValue, string? newValue);
}