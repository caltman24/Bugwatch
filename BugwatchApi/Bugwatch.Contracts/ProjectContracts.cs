namespace Bugwatch.Contracts;

// Used for reflection in validator
public abstract record ProjectRequest(string Status, string Priority);

public record NewProjectRequest(
    Guid? ProjectManagerId,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime? Deadline) : ProjectRequest(Status, Priority);

public record UpsertProjectRequest(
    Guid? ProjectManagerId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? Deadline) : ProjectRequest(Status, Priority);