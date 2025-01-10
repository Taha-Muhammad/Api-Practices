using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
	public class CourseForUpdateDto : CourseForManipulationDto
	{
		[Required(ErrorMessage = "You should fill out a description.")]
		public override string Description
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
		{ get => base.Description!; set => base.Description = value; }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
	}
}
