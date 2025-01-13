using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models.AuthorDtos;
public abstract class AuthorForManipulationDto : IValidatableObject
{
	[Required(ErrorMessage = "You should fill out a first name.")]
	[MaxLength(20,
		ErrorMessage = "The first name shouldn't have more than 20 characters.")]
	public virtual string FirstName { get; set; } = string.Empty;
	[Required(ErrorMessage = "You should fill out a last name.")]
	[MaxLength(20,
		ErrorMessage = "The last name shouldn't have more than 20 characters.")]
	public virtual string LastName { get; set; } = string.Empty;
	[Required(ErrorMessage = "You should fill out a date of birth.")]
	public virtual DateTimeOffset DateOfBirth { get; set; }
	[Required(ErrorMessage = "You should fill out a main category.")]
	[MaxLength(50,
		ErrorMessage = "The main category shouldn't have more than 50 characters.")]
	public virtual string MainCategory { get; set; } = string.Empty;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (DateOfBirth > DateTimeOffset.UtcNow.AddYears(-10))
		{
			yield return new ValidationResult(
				"The author must be older than 10 years old.",
				["author"]);
		}
	}
}
