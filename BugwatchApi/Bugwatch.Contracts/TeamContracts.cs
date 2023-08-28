namespace Bugwatch.Contracts;

public record NewTeamRequest(string Name, Guid CreatorId);

public record UpdateTeamRequest(string Name);

public record GetTeamResponse(
    Guid Id,
    Guid CreatorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);