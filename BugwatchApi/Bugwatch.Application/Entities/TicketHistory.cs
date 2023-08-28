using Bugwatch.Application.ValueObjects;

namespace Bugwatch.Application.Entities;

public class TicketHistory : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public TeamMemberInfo TeamMember { get; set; }
    public string Message { get; set; }
    public string EventName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}