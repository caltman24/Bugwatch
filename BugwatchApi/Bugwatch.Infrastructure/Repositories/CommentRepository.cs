using Bugwatch.Application.Entities;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly string _connectionString;

    public CommentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<TicketComment?> GetByIdAsync(Guid commentId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"
            SELECT tc.* FROM ticket_comment tc
            WHERE tc.id = @commentId;";

        return await conn.QueryFirstOrDefaultAsync<TicketComment>(sql, new { commentId });
    }

    public async Task AddAsync(TicketComment newTicketComment)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO ticket_comment (id, creator_id, description, created_at, updated_at, ticket_id)
            VALUES (@Id, @CreatorId, @Description, @CreatedAt, @UpdatedAt, @TicketId);";

        await conn.ExecuteAsync(sql, new
        {
            newTicketComment.Id,
            newTicketComment.CreatorId,
            newTicketComment.Description,
            newTicketComment.CreatedAt,
            newTicketComment.UpdatedAt,
            newTicketComment.TicketId
        });
    }

    public async Task DeleteAsync(Guid commentId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = "DELETE FROM ticket_comment tc WHERE tc.id = @commentId;";

        await conn.ExecuteAsync(sql, new { commentId });
    }
}

public interface ICommentRepository
{
    Task<TicketComment?> GetByIdAsync(Guid commentId);
    Task AddAsync(TicketComment newTicketComment);
    Task DeleteAsync(Guid commentId);
}