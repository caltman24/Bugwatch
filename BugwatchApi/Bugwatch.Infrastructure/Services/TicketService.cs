using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;

namespace Bugwatch.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ITicketHistoryService _ticketHistoryService;
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketHistoryService ticketHistoryService, ITicketRepository ticketRepository)
    {
        _ticketHistoryService = ticketHistoryService;
        _ticketRepository = ticketRepository;
    }

    public Task UpdateWithHistoryAsync(Guid ticketId, BasicTicket ticket, string authId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateWithHistoryAsync(BasicTicket newTicket, string authId)
    {
        var ticketHistory = _ticketHistoryService.CreateNewInstance(newTicket.Id);

        await _ticketRepository.InsertAsync(newTicket, ticketHistory, authId);
    }
}