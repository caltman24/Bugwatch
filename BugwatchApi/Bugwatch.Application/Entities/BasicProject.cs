namespace Bugwatch.Application.Entities;

/// <summary>
///     Project entity with no tickets or members
/// </summary>
public class BasicProject : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid? ProjectManagerId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public DateTime? Deadline { get; set; }
}

// Project/Id PAGE
// Send request to API

// Query for all tickets for project -> array of tickets
// -> array of ticketIds

// Query for all assigned members in a project -> array of TeamMembers
// SELECT DISTINCT tm.id, tm.name
// FROM team_member tm
// JOIN ticket t on tm.id = t.team_member_id
// WHERE t.project_id = @projectId
// AND t.id IN (@ticketIds) // array of ids

// Return Project/id View Model