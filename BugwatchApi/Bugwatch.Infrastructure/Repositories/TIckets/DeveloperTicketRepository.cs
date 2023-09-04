using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Tickets;

public class DeveloperTicketRepository : IRoleTicketRepository
{
    private readonly DapperContext _dapperContext;

    public DeveloperTicketRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IQueryable<BasicTicket>> GetTicketsAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT t.* FROM ticket t 
                INNER JOIN team_member tm on tm.id = t.developer_id
                INNER JOIN ""user"" u on u.id = tm.user_id
            WHERE u.auth_id = @authId;";

        return (await conn.QueryAsync<BasicTicket>(sql, new { authId })).AsQueryable();
    }
}