namespace Bugwatch.Contracts;

public record UpsertCommentRequest(
    Guid? CommentId,
    string Description);