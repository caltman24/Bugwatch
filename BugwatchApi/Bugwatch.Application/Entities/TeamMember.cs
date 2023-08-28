using System.ComponentModel.DataAnnotations;

namespace Bugwatch.Application.Entities;

public class TeamMember : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public string Role { get; set; }
    public string Name { get; set; }
    [MaxLength(30)] public string Username { get; set; }
}