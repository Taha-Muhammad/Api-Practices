﻿namespace CourseLibrary.API.Models.AuthorDtos
{
	public class FullAuthorDto
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public DateTimeOffset DateOfBirth { get; set; }
		public string MainCategory { get; set; } = string.Empty;
	}
}
