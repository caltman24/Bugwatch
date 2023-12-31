﻿using System.Text;
using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Application.ValueObjects;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories.Tickets;

public class TicketRepository : ITicketRepository
{
    private readonly DapperContext _dapperContext;

    public TicketRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IQueryable<BasicTicket>> GetAllAsync(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT ticket.* FROM ticket
                INNER JOIN project p on p.id = ticket.project_id
                INNER JOIN team on p.team_id = team.id
                INNER JOIN public.team_member tm on team.id = tm.team_id
                INNER JOIN ""user"" u on tm.user_id = u.id
            WHERE u.auth_id = @authId;";

        return (await conn.QueryAsync<BasicTicket>(sql, new { authId })).AsQueryable();
    }

    public async Task<IQueryable<BasicTicket>> GetProjectTicketsAsync(Guid projectId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT t.* FROM ticket t 
            WHERE t.project_id = @projectId;";

        return (await conn.QueryAsync<BasicTicket>(sql, new { projectId })).AsQueryable();
    }

    public async Task<Ticket?> GetByIdAsync(Guid ticketId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            SELECT t.*, tc.*, th.id, th.ticket_id, th.event_name, th.message,
                   th.old_value, th.new_value, th.created_at, th.updated_at,
                   tm.id, tm.name, tm.username, tm.role 
            FROM ticket t 
                LEFT JOIN ticket_comment tc on t.id = tc.ticket_id
                LEFT JOIN ticket_history th on t.id = th.ticket_id
                LEFT JOIN team_member tm on th.team_member_id = tm.id
            WHERE t.id = @ticketId;";

        List<TicketComment> comments = new();
        List<TicketHistory> ticketHistory = new();

        var result =
            await conn.QueryAsync<Ticket, TicketComment, TicketHistory, TeamMemberInfo, Ticket>(sql,
                (ticket, comment, history, teamMemberInfo) =>
                {
                    ticket.Comments = comments;

                    if (comment != null) comments.Add(comment);

                    if (history == null) return ticket;

                    history.TeamMember = teamMemberInfo;
                    ticket.TicketHistory = ticketHistory;
                    ticketHistory.Add(history);

                    return ticket;
                }, new { ticketId }, splitOn: "id");

        return result.FirstOrDefault();
    }

    public async Task InsertAsync(BasicTicket newTicket, TicketHistory? ticketHistory, string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        var sb = new StringBuilder(@"");

        var sql = @"
            INSERT INTO ticket (id, project_id, developer_id, 
                                title, description, status, 
                                type, priority, created_at, 
                                updated_at, submitter_id)
            VALUES (@Id, @ProjectId, @DeveloperId, 
                    @Title, @Description, @Status, 
                    @Type, @Priority, @CreatedAt, 
                    @UpdatedAt, @SubmitterId);
            ";

        if (ticketHistory != null)
            sql += @"
                INSERT INTO ticket_history (id, ticket_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at, team_member_id) 
                SELECT @HistoryId, @Id, @HistoryMessage, 
                        @EventName, @OldValue, @NewValue, 
                        @CreatedAt, @UpdatedAt, tm.id
                FROM team_member tm
                INNER JOIN ""user"" u on tm.user_id = u.id
                WHERE u.auth_id = @authId;";

        await conn.ExecuteAsync(sql, new
        {
            newTicket.Id,
            newTicket.ProjectId,
            newTicket.DeveloperId,
            newTicket.Title,
            newTicket.Description,
            newTicket.Status,
            newTicket.Type,
            newTicket.Priority,
            newTicket.CreatedAt,
            newTicket.UpdatedAt,
            newTicket.SubmitterId,
            HistoryId = ticketHistory?.Id,
            HistoryMessage = ticketHistory?.Message,
            ticketHistory?.EventName,
            ticketHistory?.OldValue,
            ticketHistory?.NewValue,
            authId
        });
    }

    public async Task UpdateAsync(Guid ticketId, BasicTicket updatedTicket)
    {
        using var conn = _dapperContext.CreateConnection();

        var sql = @"
            UPDATE ticket SET
                developer_id = @DeveloperId,
                title = @Title,
                description = @Description,
                status = @Status,
                type = @Type,
                priority = @Priority,
                updated_at = @UpdatedAt
            WHERE ticket.id = @ticketId;";

        await conn.ExecuteAsync(sql, new
        {
            updatedTicket.DeveloperId,
            updatedTicket.Title,
            updatedTicket.Description,
            updatedTicket.Status,
            updatedTicket.Type,
            updatedTicket.Priority,
            updatedTicket.UpdatedAt,
            ticketId,
        });
    }

    public async Task UpdateStatusAsync(Guid ticketId, string status, TicketHistory? ticketHistory, string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        var sql = @"
            UPDATE ticket SET
                status = @status
            WHERE ticket.id = @ticketId;";

        if (ticketHistory != null)
            sql += @"
                INSERT INTO ticket_history (id, ticket_id, message, 
                                            event_name, old_value, new_value, 
                                            created_at, updated_at, team_member_id) 
                SELECT @HistoryId, @Id, @HistoryMessage, 
                        @EventName, @OldValue, @NewValue, 
                        @CreatedAt, @UpdatedAt, tm.id
                FROM team_member tm
                INNER JOIN ""user"" u on tm.user_id = u.id
                WHERE u.auth_id = @authId;";

        await conn.ExecuteAsync(sql, new
        {
            status,
            ticketId,
            HistoryId = ticketHistory?.Id,
            HistoryMessage = ticketHistory?.Message,
            ticketHistory?.EventName,
            ticketHistory?.OldValue,
            ticketHistory?.NewValue,
            ticketHistory?.CreatedAt,
            ticketHistory?.UpdatedAt
        });
    }

    public async Task UpdateDeveloperAsync(Guid ticketId, Guid developerId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            UPDATE ticket set 
               developer_id = @developerId
            WHERE id = @ticketId;";

        await conn.ExecuteAsync(sql, new { ticketId, developerId });
    }

    public async Task DeleteAsync(Guid ticketId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            DELETE FROM ticket
            WHERE ticket.id = @ticketId";

        await conn.ExecuteAsync(sql, new { ticketId });
    }
}