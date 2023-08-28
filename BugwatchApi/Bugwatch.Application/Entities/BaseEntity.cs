namespace Bugwatch.Application.Entities;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}