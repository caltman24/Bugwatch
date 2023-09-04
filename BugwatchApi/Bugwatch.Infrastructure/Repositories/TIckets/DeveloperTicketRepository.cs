using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Tickets;

public class DeveloperTicketRepository : IRoleTicketRepository
{
    private readonly string _connectionString;

    public DeveloperTicketRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IQueryable<BasicTicket>> GetTicketsAsync(string authId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = @"
            SELECT t.* FROM ticket t 
                INNER JOIN team_member tm on tm.id = t.developer_id
                INNER JOIN ""user"" u on u.id = tm.user_id
            WHERE u.auth_id = @authId;";

        return (await conn.QueryAsync<BasicTicket>(sql, new { authId })).AsQueryable();
    }
}