using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface IRoleTicketRepository
{
    Task<IQueryable<BasicTicket>> GetTicketsAsync(string authId);
}