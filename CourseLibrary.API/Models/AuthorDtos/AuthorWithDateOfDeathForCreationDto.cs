namespace CourseLibrary.API.Models.AuthorDtos
{
	public class AuthorWithDateOfDeathForCreationDto : AuthorForManipulationDto
	{
		public DateTimeOffset? DateOfDeath { get; set; }
	}
}
