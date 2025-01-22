using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent
{
	public class GetCategoriesListWithEventsQueryHandler : IRequestHandler<GetCategoriesListWithEventsQuery
		, List<CategoryEventListVm>>
	{
		private readonly IMapper _mapper;
		private readonly ICategoryRepository _categoryRepository;

		public GetCategoriesListWithEventsQueryHandler(IAsyncRepository<Event> eventRepository,
			ICategoryRepository categoryRepository,
			IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<List<CategoryEventListVm>> Handle(GetCategoriesListWithEventsQuery request,
			CancellationToken cancellationToken)
		{
			var categoriesEntities = await _categoryRepository
				.GetCategoriesWithEventsAsync(request.IncludeHistory);
			return _mapper.Map<List<CategoryEventListVm>>(categoriesEntities);
		}
	}
}
