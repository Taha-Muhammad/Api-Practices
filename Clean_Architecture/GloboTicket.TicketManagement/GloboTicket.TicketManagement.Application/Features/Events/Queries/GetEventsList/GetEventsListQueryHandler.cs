using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventsList
{
	public class GetEventsListQueryHandler : IRequestHandler<GetEventsListQuery,
		List<EventListVm>>
	{
		private readonly IMapper _mapper;
		private readonly IAsyncRepository<Event> _eventRepository;
		public GetEventsListQueryHandler(IMapper mapper,
			IAsyncRepository<Event> eventRepository)
		{
			_mapper = mapper;
			_eventRepository = eventRepository;
		}
		public async Task<List<EventListVm>> Handle(GetEventsListQuery request,
			CancellationToken cancellationToken)
		{
			var events = (await _eventRepository.ListAllAsync())
				.OrderBy(e => e.Date);

			return _mapper.Map<List<EventListVm>>(events);
		}
	}
}
