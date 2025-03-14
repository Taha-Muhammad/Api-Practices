﻿using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Entities;

public class Author
{
	public Guid Id { get; set; }
	[Required]
	[MaxLength(50)]
	public string FirstName { get; set; }
	[Required]
	[MaxLength(50)]
	public string LastName { get; set; }
	[Required]
	public DateTimeOffset DateOfBirth { get; set; }
	public DateTimeOffset? DateOfDeath { get; set; }
	[Required]
	[MaxLength(50)]
	public string MainCategory { get; set; }
	public ICollection<Course> Courses { get; set; } = [];
	public Author(string firstName, string lastName, string mainCategory)
	{
		FirstName = firstName;
		LastName = lastName;
		MainCategory = mainCategory;
	}
}
