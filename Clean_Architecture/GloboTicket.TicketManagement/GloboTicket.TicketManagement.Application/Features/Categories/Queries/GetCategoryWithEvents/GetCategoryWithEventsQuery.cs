using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoryWithEvents
{
	public class GetCategoryWithEventsQuery : IRequest<CategoryEventListVm>
	{
		public Guid CategoryId { get; set; }
	}
}
