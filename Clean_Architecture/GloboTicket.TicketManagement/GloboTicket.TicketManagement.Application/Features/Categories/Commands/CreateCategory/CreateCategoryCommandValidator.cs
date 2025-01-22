using FluentValidation;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCategory
{
	public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
	{
		public CreateCategoryCommandValidator()
		{
			RuleFor(c => c.Name).NotNull().NotEmpty()
				.WithMessage("{PropertyName} is required.")
				.MaximumLength(50)
				.WithMessage("{PropertyName} must not exceed 50 characters.");
		}
	}
}

