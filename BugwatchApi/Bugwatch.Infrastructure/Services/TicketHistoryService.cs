using Bugwatch.Application.Constants;
using Bugwatch.Application.Entities;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Repositories;

namespace Bugwatch.Infrastructure.Services;

public class TicketHistoryService : ITicketHistoryService
{
    private readonly ITicketHistoryFactory _ticketHistoryFactory;
    private readonly ITicketHistoryRepository _ticketHistoryRepository;
    private readonly ITicketRepository _ticketRepository; // used to get old ticket by id

    public TicketHistoryService(ITicketHistoryRepository ticketHistoryRepository,
        ITicketHistoryFactory ticketHistoryFactory, ITicketRepository ticketRepository
    )
    {
        _ticketHistoryRepository = ticketHistoryRepository;
        _ticketHistoryFactory = ticketHistoryFactory;
        _ticketRepository = ticketRepository;
    }

    public TicketHistory CreateNewInstance(Guid ticketId)
    {
        return _ticketHistoryFactory.CreateFromEvent(ticketId, TicketHistoryEvents.Created, null, null);
    }

    public async Task AddHistoryToTicketAsync(BasicTicket newTicket, string authId)
    {
        var oldTicket = await _ticketRepository.GetByIdAsync(newTicket.Id);

        if (oldTicket == null) return;

        ICollection<TicketHistory> ticketHistories = new List<TicketHistory>();

        if (oldTicket.Title != newTicket.Title)
        {
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewTitle,
                oldTicket.Title,
                newTicket.Title));
        }

        if (oldTicket.Description != newTicket.Description)
        {
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewDescription,
                oldTicket.Description,
                newTicket.Description));
        }

        if (oldTicket.Priority != newTicket.Priority)
        {
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewPriority,
                oldTicket.Priority,
                newTicket.Priority));
        }

        if (oldTicket.Status != newTicket.Status)
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewStatus,
                oldTicket.Status,
                newTicket.Status));

        if (oldTicket.Type != newTicket.Type)
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewType,
                oldTicket.Type,
                newTicket.Type));

        if (oldTicket.DeveloperId != newTicket.DeveloperId)
            ticketHistories.Add(_ticketHistoryFactory.CreateFromEvent(
                newTicket.Id,
                TicketHistoryEvents.NewDev,
                oldTicket.DeveloperId.ToString(),
                newTicket.DeveloperId.ToString()));

        await _ticketHistoryRepository.InsertManyAsync(ticketHistories, authId);
    }
}