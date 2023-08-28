namespace Bugwatch.Application.Entities;

public class TicketComment : BaseEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TicketId { get; set; }
    public string Description { get; set; }
}