using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class TicketHistoryRepository : ITicketHistoryRepository
{
    private readonly DapperContext _dapperContext;

    public TicketHistoryRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task InsertManyAsync(IEnumerable<TicketHistory> ticketHistories, string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"INSERT INTO ticket_history (id, ticket_id, team_member_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at)
                SELECT @Id, @TicketId, tm.id, @Message, @EventName, @OldValue, @NewValue, @CreatedAt, @UpdatedAt
                FROM team_member tm
                INNER JOIN ""user"" u on u.id = tm.user_id
                WHERE u.auth_id = @authId";
        
        // OPTIMIZE
        await conn.ExecuteAsync(sql, ticketHistories.Select(history => new
        {
            history.Id,
            history.TicketId,
            history.Message,
            history.EventName,
            history.OldValue,
            history.NewValue,
            history.CreatedAt,
            history.UpdatedAt,
            authId
        }));
    }

    public async Task InsertAsync(TicketHistory ticketHistory)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"INSERT INTO ticket_history (id, ticket_id, team_member_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at)
                VALUES (@Id, @TicketId, @TeamMemberId, @Message, @EventName, @OldValue, @NewValue, @CreatedAt, @UpdatedAt);";

        await conn.ExecuteAsync(sql, new
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