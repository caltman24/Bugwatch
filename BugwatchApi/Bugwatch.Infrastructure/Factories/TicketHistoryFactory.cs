using Bugwatch.Application.Entities;
using Bugwatch.Application.Factories;
using Bugwatch.Application.Interfaces;

namespace Bugwatch.Infrastructure.Factories;

public class TicketHistoryFactory : ITicketHistoryFactory
{
    public TicketHistory CreateFromEvent(Guid ticketId, string eventName, string? oldValue = null,
        string? newValue = null)
    {
        return new TicketHistory
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Message = TicketHistoryMessageFactory.CreateMessageForEvent(eventName, DateTime.UtcNow, oldValue, newValue),
            EventName = eventName,
            CreatedAt = DateTime.UtcNow
        };
    }
}