using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Application.ValueObjects;

namespace Bugwatch.Infrastructure.Factories;

public class TicketHistoryFactory : ITicketHistoryFactory
{
    public TicketHistory CreateFromEvent(Guid ticketId, string eventName,
        string? oldValue = null,
        string? newValue = null)
    {
        return new TicketHistory
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Message = TicketHistoryMessageFactory.CreateMessageForEvent(eventName, oldValue, newValue),
            EventName = eventName,
            CreatedAt = DateTime.UtcNow,
            OldValue = oldValue,
            NewValue = newValue,
        };
    }
}