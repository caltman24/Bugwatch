using System.ComponentModel.DataAnnotations;

namespace Bugwatch.Application.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public Guid AuthId { get; set; }
    [EmailAddress] public string Email { get; set; }
    public string Name { get; set; }
}