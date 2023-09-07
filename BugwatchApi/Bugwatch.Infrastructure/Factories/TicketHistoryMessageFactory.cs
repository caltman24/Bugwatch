using System.Globalization;
using Bugwatch.Application.Constants;

namespace Bugwatch.Infrastructure.Factories;

public static class TicketHistoryMessageFactory
{
    // TODO: Maybe remove timestamp from messages
    public static string CreateMessageForEvent(string eventName, DateTime date, string? oldValue,
        string? newValue)
    {
        return eventName switch
        {
            TicketHistoryEvents.Created => $"Ticket created at {GetDateString(date)}",
            TicketHistoryEvents.NewDev =>
                $"Assigned developer changed from ${oldValue} to ${newValue} at {GetDateString(date)}",
            TicketHistoryEvents.NewStatus =>
                $"Ticket status changed from ${oldValue} to ${newValue} at {GetDateString(date)}",
            TicketHistoryEvents.NewType =>
                $"Ticket type changed from ${oldValue} to ${newValue} at {GetDateString(date)}",
            TicketHistoryEvents.NewPriority =>
                $"Ticket priority changed from ${oldValue} to ${newValue} at {GetDateString(date)}",
            TicketHistoryEvents.NewDescription =>
                $"Ticket description changed at {GetDateString(date)}",
            TicketHistoryEvents.NewTitle =>
                $"Ticket title changed from ${oldValue} to ${newValue} at {GetDateString(date)}",
            _ => throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null)
        };
    }

    private static string GetDateString(DateTime date)
    {
        return date.ToString(CultureInfo.InvariantCulture);
    }
}