using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Projects;

public class DeveloperProjectRepository : IRoleProjectRepository
{
    private readonly DapperContext _dapperContext;

    public DeveloperProjectRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IQueryable<BasicProject>> GetProjectsAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

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