using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Dapper;
using Npgsql;

namespace Bugwatch.Application.Repositories;

public class TicketHistoryRepository : ITicketHistoryRepository
{
    private readonly string _connectionString;

    public TicketHistoryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // TODO: FIXME
    public async Task InsertManyAsync(IEnumerable<TicketHistory> ticketHistories, string authId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"INSERT INTO ticket_history (id, ticket_id, team_member_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at)
                VALUES (@Id, @TicketId, @TeamMemberId, @Message, @EventName, @OldValue, @NewValue, @CreatedAt, @UpdatedAt);";

        await conn.QueryAsync(sql, ticketHistories);
    }

    public async Task InsertAsync(TicketHistory ticketHistory)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"INSERT INTO ticket_history (id, ticket_id, team_member_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at)
                VALUES (@Id, @TicketId, @TeamMemberId, @Message, @EventName, @OldValue, @NewValue, @CreatedAt, @UpdatedAt);";

        await conn.QueryAsync(sql, new
        {
            ticketHistory.Id,
            ticketHistory.TicketId,
            TeamMemberId = ticketHistory.TeamMember.Id,
            ticketHistory.Message,
            ticketHistory.EventName,
            ticketHistory.OldValue,
            ticketHistory.NewValue,
            ticketHistory.CreatedAt,
            ticketHistory.UpdatedAt
        });
    }
}