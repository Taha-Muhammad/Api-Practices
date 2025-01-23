using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Application.Exceptions;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Events.Commands.DeleteEvent
{
	public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
	{
		private readonly IEventRepository _eventRepository;

		public DeleteEventCommandHandler(IMapper mapper,
			IEventRepository eventRepository)
		{
			_eventRepository = eventRepository;
		}

		public async Task Handle(DeleteEventCommand request,
			CancellationToken cancellationToken)
		{
			var eventToDelete  = await  _eventRepository.GetByIdAsync(request.EventId);

			if (eventToDelete == null)
				throw new NotFoundException("Event", request);

			await _eventRepository.DeleteAsync(eventToDelete);
		}
	}
}
