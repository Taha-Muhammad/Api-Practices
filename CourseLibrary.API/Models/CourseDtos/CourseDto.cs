﻿namespace CourseLibrary.API.Models.CourseDtos
{
	public class CourseDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
	}
}
