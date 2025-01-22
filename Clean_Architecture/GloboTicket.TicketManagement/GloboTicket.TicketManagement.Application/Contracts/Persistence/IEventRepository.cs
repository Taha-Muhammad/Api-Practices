using GloboTicket.TicketManagement.Domain.Entities;

namespace GloboTicket.TicketManagement.Application.Contracts.Persistence
{
	public interface IEventRepository : IAsyncRepository<Event>
	{
		public Task<bool> IsEventNameAndDateUnique(string eventName, DateTime eventDate);
	}
}
