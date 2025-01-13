namespace CourseLibrary.API.Models.AuthorDtos
{
	public class AuthorDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Age { get; set; }
		public string MainCategory { get; set; } = string.Empty;
	}
}
