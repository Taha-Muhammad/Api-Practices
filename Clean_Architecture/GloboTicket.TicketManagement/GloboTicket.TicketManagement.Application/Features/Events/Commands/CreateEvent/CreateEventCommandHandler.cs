﻿using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Application.Exceptions;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Events.Commands.CreateEvent
{
	public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
	{
		private readonly IEventRepository _eventRepository;
		private readonly IMapper _mapper;

		public CreateEventCommandHandler(IMapper mapper,
			IEventRepository eventRepository)
		{
			_mapper = mapper;
			_eventRepository = eventRepository;
		}

		public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
		{
			var eventToAdd = _mapper.Map<Event>(request);
			
			var validator = new CreateEventCommandValidator(_eventRepository);
			var validationResult =
				await validator.ValidateAsync(request, cancellationToken);

			if (validationResult.Errors.Count > 0)
				throw new ValidationException(validationResult);

			eventToAdd = await _eventRepository.AddAsync(eventToAdd);
			return eventToAdd.EventId;
		}
	}
}
