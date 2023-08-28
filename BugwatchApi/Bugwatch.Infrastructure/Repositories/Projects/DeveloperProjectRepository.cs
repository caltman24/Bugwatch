using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Dapper;
using Npgsql;

namespace Bugwatch.Application.Repositories.Projects;

public class DeveloperProjectRepository : IRoleProjectRepository
{
    private readonly string _connectionString;

    public DeveloperProjectRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IQueryable<BasicProject>> GetProjectsAsync(string authId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        return (await conn.QueryAsync<BasicProject>(@"
            SELECT DISTINCT p.id, p.team_id, p.title, 
                   p.created_at, p.updated_at, p.description, 
                   p.status, p.priority 
            FROM ""user"" u
                INNER JOIN team_member tm on u.id = tm.user_id
                INNER JOIN ticket tk on tk.developer_id = tm.id
                INNER JOIN project p on tk.project_id = p.id
                INNER JOIN team t on p.team_id = t.id
            WHERE u.auth_id = @authId",
            new { authId })).AsQueryable();
    }
}