using FluentValidation;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;

namespace GloboTicket.TicketManagement.Application.Features.Events.Commands.UpdateEvent
{
	public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
	{
		private readonly IEventRepository _eventRepository;

		public UpdateEventCommandValidator(IEventRepository eventRepository)
		{
			_eventRepository = eventRepository;
			RuleFor(e => e.Name).NotNull()
				.NotEmpty().WithMessage("{PropertyName} is required.")
				.MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

			RuleFor(e => e.Artist).NotEmpty().NotNull()
				.WithMessage("{PropertyName} is required.")
				.MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
			
			RuleFor(e => e.Description).NotEmpty().NotNull()
				.WithMessage("{PropertyName} is required.")
				.MaximumLength(1500).WithMessage("{PropertyName} must not exceed 1500 characters.");
			
			RuleFor(e => e.ImageUrl).NotEmpty().NotNull()
				.WithMessage("{PropertyName} is required.");
			
			RuleFor(e => e.Price).NotEmpty().NotNull()
				.WithMessage("{PropertyName} is required.").GreaterThan(0);
			
			RuleFor(e => e.Date).NotEmpty().NotNull()
				.WithMessage("{PropertyName} is required.").GreaterThan(DateTime.Now);
			RuleFor(e => e).MustAsync(EventNameAndDateIsUnique)
			.WithMessage("An event with the same name and date already exists.");
		}
		private async Task<bool> EventNameAndDateIsUnique(UpdateEventCommand e,
				CancellationToken cancellationToken)
		{
			return !(await _eventRepository.IsEventNameAndDateUnique(e.Name, e.Date));
		}
	}
}
