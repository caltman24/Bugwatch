namespace Bugwatch.Application.Entities;

public class Team : BaseEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; } // UserId
    public string Name { get; set; }
}