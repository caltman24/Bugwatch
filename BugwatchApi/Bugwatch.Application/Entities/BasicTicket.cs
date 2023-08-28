namespace Bugwatch.Application.Entities;

public class BasicTicket : BaseEntity
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
}

// DeveloperId (Admin, ProjectManager)
// Title & Title (Submitter, Admin, ProjectManager)
// Status (Dev, Admin, ProjectManager)
// Type (Submitter, Admin, ProjectManager)
// Priority (Submitter, Admin, ProjectManager)