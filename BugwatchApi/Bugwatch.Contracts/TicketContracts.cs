namespace Bugwatch.Contracts;

public abstract record TicketRequest(string Status, string Priority, string Type);

public record GetTicketRequest(
    Guid Id,
    Guid SubmitterId,
    Guid? DeveloperId,
    Guid ProjectId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<GetTicketHistoryRequest> TicketHistory) : TicketRequest(Status, Priority, Type);

public record NewTicketRequest(
    Guid? DeveloperId,
    Guid ProjectId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    string Status) : TicketRequest(Status, Priority, Type);

public record UpdateTicketStatusRequest(
    Guid TicketId,
    string Status);

public record UpdateTicketRequest(
    Guid? DeveloperId,
    string Title,
    string? Description,
    string Priority,
    string Type, string Status) : TicketRequest(Status, Priority, Type);