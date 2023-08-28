using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITicketHistoryRepository
{
    Task InsertManyAsync(IEnumerable<TicketHistory> ticketHistories, string authId);
    Task InsertAsync(TicketHistory ticketHistory);
}