using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Aggregates;

public class Ticket : BaseEntity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? DeveloperId { get; set; }
    public Guid SubmitterId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public string Priority { get; set; }
    public List<TicketComment> Comments { get; set; } = new();
    public List<TicketHistory> TicketHistory { get; set; } = new();
}