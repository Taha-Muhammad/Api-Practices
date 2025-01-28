using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Application.Exceptions;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Events.Commands.UpdateEvent
{
	public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
	{
		private readonly IEventRepository _eventRepository;
		private readonly IMapper _mapper;

		public UpdateEventCommandHandler(IMapper mapper,
			IEventRepository eventRepository)
		{
			_mapper = mapper;
			_eventRepository = eventRepository;
		}

		public async Task Handle(UpdateEventCommand request,
			CancellationToken cancellationToken)
		{
			var eventToUpdate = await _eventRepository.GetByIdAsync(request.EventId);
			
			if (eventToUpdate == null)
				throw new NotFoundException("Event",request.EventId);
			
			var validator = 
				new UpdateEventCommandValidator(_eventRepository);
			var validationResult =
				await validator.ValidateAsync(request, cancellationToken);

			if (validationResult.Errors.Count > 0)
				throw new ValidationException(validationResult);

			_mapper.Map(request,eventToUpdate);

			await _eventRepository.UpdateAsync(eventToUpdate);
		}
	}
}
