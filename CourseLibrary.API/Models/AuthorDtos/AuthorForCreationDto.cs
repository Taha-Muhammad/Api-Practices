using CourseLibrary.API.Models.CourseDtos;

namespace CourseLibrary.API.Models.AuthorDtos
{
	public class AuthorForCreationDto : AuthorForManipulationDto
	{
		public ICollection<CourseForCreationDto> Courses { get; set; }
			= new List<CourseForCreationDto>();
	}
}
