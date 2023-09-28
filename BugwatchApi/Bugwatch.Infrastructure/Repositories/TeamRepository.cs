using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly DapperContext _dapperContext;

    public TeamRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<Team?> GetByAuthIdAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT DISTINCT t.* FROM ""user"" u
                INNER JOIN team_member tm on u.id = tm.user_id
                INNER JOIN team t on tm.team_id = t.id
            WHERE u.auth_id = @authId";

        return await conn.QueryFirstOrDefaultAsync<Team>(sql, new { authId });
    }

    public async Task<Team?> GetByUserIdAsync(Guid userId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT DISTINCT t.* FROM team_member tm
                INNER JOIN team t on t.id = tm.team_id
            WHERE tm.user_id = @userId";

        return await conn.QueryFirstOrDefaultAsync<Team>(sql, new { userId });
    }

    public async Task InsertAsync(Team team, string authId)
    {
        using var conn = _dapperContext.CreateConnection();
        
        // TODO: Check if user is part of team. If so, return false

        const string sql = @"
            INSERT INTO team (id, creator_id, created_at, updated_at, name) 
            SELECT @Id, 
                   u.id, 
                   @CreatedAt, 
                   @UpdatedAt, 
                   @Name
            FROM ""user"" u
            WHERE u.auth_id = @authId";

        await conn.ExecuteAsync(sql, new
        {
            team.Id,
            team.CreatorId,
            team.CreatedAt,
            team.UpdatedAt,
            team.Name
        });
    }

    public async Task UpdateAsync(Guid teamId, string name)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            UPDATE team SET
                name = @name,
                updated_at = @updatedAt
            WHERE team.id = @teamId;";

        await conn.ExecuteAsync(sql, new
        {
            name,
            updatedAt = DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(Guid teamId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"DELETE from team t WHERE t.id = @teamId;";

        await conn.ExecuteAsync(sql, new { teamId });
    }
}