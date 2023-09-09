using Bugwatch.Application.Entities;
using Bugwatch.Application.ValueObjects;

namespace Bugwatch.Application.Interfaces;

public interface ITicketHistoryFactory
{
    TicketHistory CreateFromEvent(Guid ticketId, string eventName, string? oldValue, string? newValue);
}