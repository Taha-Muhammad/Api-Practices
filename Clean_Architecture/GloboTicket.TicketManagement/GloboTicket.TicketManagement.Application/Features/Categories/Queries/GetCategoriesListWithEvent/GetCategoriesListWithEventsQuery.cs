using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent
{
	public class GetCategoriesListWithEventsQuery : IRequest<List<CategoryEventListVm>>
	{
		public bool IncludeHistory { get; set; }
	}
}
