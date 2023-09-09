using System.Globalization;
using Bugwatch.Application.Constants;

namespace Bugwatch.Infrastructure.Factories;

public static class TicketHistoryMessageFactory
{
    public static string CreateMessageForEvent(string eventName, string? oldValue,
        string? newValue)
    {
        return eventName switch
        {
            TicketHistoryEvents.Created => $"Ticket created",
            TicketHistoryEvents.NewDev =>
                $"Assigned developer changed from {oldValue} to {newValue}",
            TicketHistoryEvents.NewStatus =>
                $"Ticket status changed from {oldValue} to {newValue}",
            TicketHistoryEvents.NewType =>
                $"Ticket type changed from {oldValue} to {newValue}",
            TicketHistoryEvents.NewPriority =>
                $"Ticket priority changed from {oldValue} to {newValue}",
            TicketHistoryEvents.NewDescription =>
                $"Ticket description changed",
            TicketHistoryEvents.NewTitle =>
                $"Ticket title changed from {oldValue} to {newValue}",
            _ => throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null)
        };
    }
}