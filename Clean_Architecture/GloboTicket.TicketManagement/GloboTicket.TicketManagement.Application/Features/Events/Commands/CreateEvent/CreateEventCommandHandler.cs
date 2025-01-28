using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Infrastructure;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Application.Exceptions;
using GloboTicket.TicketManagement.Application.Models.Mail;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
namespace GloboTicket.TicketManagement.Application.Features.Events.Commands.CreateEvent
{
	public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
	{
		private readonly IEventRepository _eventRepository;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
		private readonly ILogger<CreateEventCommandHandler> _logger;

		public CreateEventCommandHandler(IMapper mapper,
			IEventRepository eventRepository,
			IEmailService emailService,
			ILogger<CreateEventCommandHandler> logger)
		{
			_mapper = mapper;
			_eventRepository = eventRepository;
			_emailService = emailService;
			_logger = logger;
		}

		public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
		{
			if(!await _eventRepository.CategoryExists(request.CategoryId))
				throw new NotFoundException(nameof(request.CategoryId),request.CategoryId);
			var eventToAdd = _mapper.Map<Event>(request);
			
			var validator = new CreateEventCommandValidator(_eventRepository);
			var validationResult =
				await validator.ValidateAsync(request, cancellationToken);

			if (validationResult.Errors.Count > 0)
				throw new ValidationException(validationResult);

			eventToAdd = await _eventRepository.AddAsync(eventToAdd);

			var email = new Email("example@example.com",
				"A new event was created.",
				$"A new event was created: {request}.");
			try
			{
				await _emailService.SendEmailAsync(email);
			}
			catch (Exception ex) 
			{
				_logger.LogError("Something went wrong when with sending the email {massage}",
					ex.Message);
			}
			return eventToAdd.EventId;
		}
	}
}
