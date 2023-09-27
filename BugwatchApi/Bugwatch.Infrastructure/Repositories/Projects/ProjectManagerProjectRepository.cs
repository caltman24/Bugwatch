using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Projects;

public class ProjectManagerProjectRepository : IRoleProjectRepository
{
    private readonly DapperContext _dapperContext;

    public ProjectManagerProjectRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IQueryable<BasicProject>> GetProjectsAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        return (await conn.QueryAsync<BasicProject>(@"
            SELECT p.id, p.team_id, p.title, 
                   p.created_at, p.updated_at, p.description, 
                   p.status, p.priority, p.project_manager_id
            FROM ""user"" u
                INNER JOIN team_member tm on u.id = tm.user_id
                INNER JOIN project p on tm.id = p.project_manager_id
                INNER JOIN team t on p.team_id = t.id
            WHERE u.auth_id = @authId",
            new { authId })).AsQueryable();
    }

    public async Task<bool> HasTicketToAssignedProjects(Guid ticketId, string authId)
    {
        // TODO: Look this over. I rushed this
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
                SELECT 1
                FROM ticket t
                LEFT JOIN project p on t.project_id = p.id
                LEFT JOIN team_member tm on p.project_manager_id = tm.id
                LEFT JOIN ""user"" u on tm.user_id = u.id
                WHERE u.auth_id = @authId
                AND t.id = @ticketId;";

        var res = await conn.ExecuteScalarAsync<int>(sql, new { authId, ticketId});

        return res is 1;
    }
}