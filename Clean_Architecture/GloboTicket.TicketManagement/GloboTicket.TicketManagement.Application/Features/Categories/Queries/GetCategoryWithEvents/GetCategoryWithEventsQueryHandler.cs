using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Application.Exceptions;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoryWithEvents
{
	public class GetCategoryWithEventsQueryHandler(ICategoryRepository categoryRepository,
		IMapper mapper) : IRequestHandler<GetCategoryWithEventsQuery,
		CategoryEventListVm>
	{
		private readonly ICategoryRepository _categoryRepository = categoryRepository;
		private readonly IMapper _mapper = mapper;
		public async Task<CategoryEventListVm> Handle(GetCategoryWithEventsQuery request,
			CancellationToken cancellationToken)
		{
			return _mapper.Map<CategoryEventListVm>(await _categoryRepository
				.GetCategoryWithEventsAsync(request.CategoryId)??
				throw new NotFoundException(nameof(request.CategoryId),request.CategoryId));
		}
	}
}
