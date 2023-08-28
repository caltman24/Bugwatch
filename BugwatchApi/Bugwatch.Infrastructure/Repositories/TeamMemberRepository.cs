using Dapper;
using Npgsql;

namespace Bugwatch.Application.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly string _connectionString;

    public TeamMemberRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string?> GetRoleAsync(string authId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

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