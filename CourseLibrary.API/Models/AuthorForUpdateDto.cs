﻿namespace CourseLibrary.API.Models
{
	public class AuthorForUpdateDto
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public DateTimeOffset DateOfBirth { get; set; }
		public string MainCategory { get; set; } = string.Empty;
	}
}
