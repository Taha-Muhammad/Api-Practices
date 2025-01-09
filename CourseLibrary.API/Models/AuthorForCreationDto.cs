namespace CourseLibrary.API.Models
{
	public class AuthorForCreationDto : AuthorForManipulationDto
	{
		public ICollection<CourseForCreationDto> Courses { get; set; } 
			= new List<CourseForCreationDto>();

	}
}
