using Bugwatch.Application.ValueObjects;

namespace Bugwatch.Contracts;

public record GetTicketHistoryRequest(
    Guid Id,
    Guid TicketId,
    string Message,
    string EventName,
    string? OldValue,
    string? NewValue,
    TeamMemberInfo TeamMember);