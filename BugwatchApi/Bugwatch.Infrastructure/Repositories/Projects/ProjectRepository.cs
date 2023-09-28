using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Projects;

public class ProjectRepository : IProjectRepository
{
    private readonly DapperContext _dapperContext;

    public ProjectRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IQueryable<BasicProject>> GetAllAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        return (await conn.QueryAsync<BasicProject>(@"
            SELECT p.id, p.team_id, p.title, 
                   p.created_at, p.updated_at, p.description,
                   p.priority, p.status, p.project_manager_id 
            FROM team t
                INNER JOIN project p on t.id = p.team_id
                INNER JOIN team_member tm on t.id = tm.team_id
                INNER JOIN ""user"" u on tm.user_id = u.id
            WHERE u.auth_id = @authId",
            new { authId })).AsQueryable();
    }

    public async Task<Project?> GetByIdAsync(Guid projectId)
    {
        using var conn = _dapperContext.CreateConnection();

        /*
         * Multi-Mapping (One to Many)
         * https://dappertutorial.net/result-multi-mapping
         *
         * When querying for a project and the tickets associated to it, we have two options
         * 1. Multiple Queries
         * 2. Multi-Mapping with dapper
         *
         * When selecting a single project and joining the tickets, dapper will return only one ticket with the project
         *
         * To get all of the tickets and the project itself in a single query, in the mapping scope set the project tickets
         * reference to that of a cache outside the scope and add the ticket from the mapping scope to the cache. In this
         * case, the cache is a new List of tickets
         *
         */
        List<Ticket> tickets = new();

        var res = await conn.QueryAsync<Project, Ticket, Project>(@"
                SELECT p.*, t.* FROM project p
                    LEFT JOIN ticket t on p.id = t.project_id
                WHERE p.id = @projectId", (project, ticket) =>
        {
            if (ticket == null) return project;

            project.Tickets = tickets;
            tickets.Add(ticket);

            return project;
        }, new { projectId }, splitOn: "id");

        return res.FirstOrDefault();
    }

    public async Task InsertAsync(BasicProject project)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            INSERT INTO project (id, team_id, title, 
                                 description, created_at, updated_at, 
                                 status, priority, project_manager_id, deadline)
            VALUES (@Id, @TeamId, @Title, 
                    @Description, @CreatedAt, @UpdatedAt, 
                    @Status, @Priority, @ProjectManagerId, @Deadline)";

        await conn.ExecuteAsync(sql, new
        {
            project.Id,
            project.TeamId,
            project.Title,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt,
            project.Status,
            project.Priority,
            project.ProjectManagerId,
            project.Deadline
        });
    }

    public async Task UpdateAsync(Guid projectId, BasicProject updated)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            UPDATE project SET
                title = @Title,
                description = @Description,
                updated_at = @UpdatedAt,
                status = @Status,
                priority = @Priority,
                project_manager_id = @ProjectManagerId,
                deadline = @Deadline
             WHERE project.id = @projectId;";

        await conn.ExecuteAsync(sql, new
        {
            projectId,
            updated.Title,
            updated.Description,
            updated.UpdatedAt,
            updated.Status,
            updated.Priority,
            updated.Deadline,
            updated.ProjectManagerId
        });
    }

    public async Task AssignProjectManagerAsync(Guid projectId, Guid projectMangerId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            UPDATE project
            SET project_manager_id = @projectManagerId
            WHERE project.id = @projectId;";

        await conn.ExecuteAsync(sql, new { projectId });
    }

    public async Task UnassignProjectManagerAsync(Guid projectId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            UPDATE project
            SET project_manager_id = null
            WHERE project.id = @projectId;";

        await conn.ExecuteAsync(sql, new { projectId });
    }

    public async Task DeleteAsync(Guid projectId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"DELETE FROM project p WHERE p.id = @projectId";

        await conn.ExecuteAsync(sql, new { projectId });
    }
}