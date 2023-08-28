using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Aggregates;

/// <summary>
///     Project entity with tickets and members
/// </summary>
public class Project : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid? ProjectManagerId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public DateTime? Deadline { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}