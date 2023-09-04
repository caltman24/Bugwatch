using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly DapperContext _dapperContext;

    public TeamMemberRepository( DapperContext dapperContext)
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
}

public interface ITeamMemberRepository
{
    Task<string?> GetRoleAsync(string authId);
}