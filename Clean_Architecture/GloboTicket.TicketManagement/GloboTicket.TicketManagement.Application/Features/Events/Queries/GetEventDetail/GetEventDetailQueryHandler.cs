using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventDetail
{
	public class GetEventDetailQueryHandler : IRequestHandler<GetEventDetailQuery,
		EventDetailVm>
	{
		private readonly IMapper _mapper;
		private readonly IAsyncRepository<Event> _eventRepository;
		private readonly IAsyncRepository<Category> _categoryRepository;
		public GetEventDetailQueryHandler(IMapper mapper,
			IAsyncRepository<Event> eventRepository,
			IAsyncRepository<Category> categoryRepository)
		{
			_mapper = mapper;
			_eventRepository = eventRepository;
			_categoryRepository = categoryRepository;
		}
		public async Task<EventDetailVm> Handle(GetEventDetailQuery request,
			CancellationToken cancellationToken)
		{
			var eventEntity = await _eventRepository.GetByIdAsync(request.Id);

			var eventVm = _mapper.Map<EventDetailVm>(eventEntity);

			var category = await _categoryRepository
				.GetByIdAsync(eventEntity.CategoryId);
			eventVm.Category = _mapper.Map<CategoryDto>(category);

			return eventVm;
		}
	}
}
