using GloboTicket.TicketManagement.Domain.Entities;

namespace GloboTicket.TicketManagement.Application.Contracts.Persistence
{
	public interface IOrderRepository : IAsyncRepository<Order>
	{
		public Task<List<Order>> GetPagedOrdersForMonth(DateTime date, int page, int size);
		public Task<int> GetTotalCountOfOrdersForMonth(DateTime date);
	}
}
