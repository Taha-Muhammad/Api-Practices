using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services.PropertiesMappingDictionaries;
using CourseLibrary.API.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.Repositories;

public class CourseRepository : ICourseRepository
{
	private readonly CourseLibraryDbContext _context;
	private readonly PropertyMapping<Course> _courseMappingDictionary;

	public CourseRepository(CourseLibraryDbContext context,
		PropertyMapping<Course> courseMappingDictionary)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_courseMappingDictionary = courseMappingDictionary;
	}
	public async Task<bool> AuthorExistsAsync(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		return await _context.Authors.AnyAsync(a => a.Id == authorId);
	}
	public async Task<PagedList<Course>> GetCoursesAsync(Guid authorId,
		CoursesResourceParameters resourceParameters)
	{
		ArgumentNullException.ThrowIfNull(resourceParameters);

		var courseCollection = _context.Courses
			.Where(c=>c.AuthorId == authorId);

		if (!string.IsNullOrWhiteSpace(resourceParameters.Title))
		{
			var title = resourceParameters.Title.Trim();
			courseCollection = courseCollection.Where(c =>
			c.Title.Contains(title));
		}
		if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
		{
			var searchQuery = resourceParameters.SearchQuery.Trim();
			courseCollection = courseCollection.Where(c =>
			c.Title.Contains(searchQuery) ||
			c.Description != null && c.Description.Contains(searchQuery));
		}
		if (!string.IsNullOrWhiteSpace(resourceParameters.OrderBy))
		{
			courseCollection = courseCollection
				.ApplySort(resourceParameters.OrderBy,
			_courseMappingDictionary);
		}

		return await PagedList<Course>.CreateAsync(courseCollection,
			resourceParameters.PageNumber
			, resourceParameters.PageSize);
	}
	public async Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		return await _context.Courses
					.Where(c => c.AuthorId == authorId)
					.OrderBy(c => c.Title).ToListAsync();
	}
	public async Task<Course?> GetCourseAsync(Guid authorId, Guid courseId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		if (courseId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(courseId));
		}

		return await _context.Courses
		  .FirstOrDefaultAsync(c => c.AuthorId == authorId && c.Id == courseId);
	}
	public void AddCourse(Guid authorId, Course course)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		ArgumentNullException.ThrowIfNull(course);

		course.AuthorId = authorId;
		_context.Courses.Add(course);
	}
	public void UpdateCourse(Course course)
	{
		//no need for implementation it's just for clarification
	}
	public void DeleteCourse(Course course)
	{
		ArgumentNullException.ThrowIfNull(course);
		_context.Courses.Remove(course);
	}
	public async Task<bool> SaveAsync()
	{
		return await _context.SaveChangesAsync() >= 0;
	}

}
