namespace Bugwatch.Contracts;

public record GetTicketRequest(
    Guid SubmitterId,
    Guid? DeveloperId,
    Guid ProjectId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    string Status,
    IEnumerable<GetTicketHistoryRequest> TicketHistory);

public record NewTicketRequest(
    Guid SubmitterId,
    Guid? DeveloperId,
    Guid ProjectId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    string Status);

public record UpdateTicketStatusRequest(
    Guid TicketId,
    string Status);

public record UpdateTicketRequest(
    Guid Id,
    Guid? DeveloperId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    string Status);