namespace Bugwatch.Application.ValueObjects;

public sealed class TeamMemberInfo
{
    public TeamMemberInfo(Guid id, string name, string username, string role)
    {
        Id = id;
        Name = name;
        Username = username;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}