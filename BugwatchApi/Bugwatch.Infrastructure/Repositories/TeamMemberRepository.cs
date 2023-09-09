using Bugwatch.Application.Entities;
using Bugwatch.Application.ValueObjects;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly DapperContext _dapperContext;

    public TeamMemberRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<string?> GetRoleAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT tm.role FROM team_member tm
            INNER JOIN ""user"" u on u.id = tm.user_id
            WHERE u.auth_id = @authId;";

        return await conn.ExecuteScalarAsync<string?>(sql, new { authId });
    }

    public async Task<TeamMemberInfo> GetInfoAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT tm.id, tm.name, tm.username, tm.role FROM team_member tm
            INNER JOIN ""user"" u on u.id = tm.user_id
            WHERE u.auth_id = @authId;";

        return await conn.QueryFirstOrDefaultAsync<TeamMemberInfo>(sql, new { authId });
    }

    public async Task<bool> UserExistsAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT 1
            FROM ""user"" u
            WHERE u.auth_id = @authId;";

        var result = await conn.ExecuteScalarAsync<int?>(sql, new { authId });

        return result is 1;
    }

    public async Task<bool> ValidateRoleAsync(Guid id, string role)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT 1
            FROM team_member tm
            WHERE tm.id = @id
            AND tm.role = @role;";

        var result = await conn.ExecuteScalarAsync<int?>(sql, new { id, role });

        return result is 1;
    }
}

public interface ITeamMemberRepository
{
    Task<string?> GetRoleAsync(string authId);
    Task<TeamMemberInfo> GetInfoAsync(string authId);
    Task<bool> UserExistsAsync(string authId);
    Task<bool> ValidateRoleAsync(Guid id, string role);
}