using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services;

public class CourseLibraryRepository : ICourseLibraryRepository
{
	private readonly CourseLibraryDbContext _context;

	public CourseLibraryRepository(CourseLibraryDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
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

	public void DeleteCourse(Course course)
	{
		ArgumentNullException.ThrowIfNull(course);
		_context.Courses.Remove(course);
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
		  .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefaultAsync();
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
	public async Task<PagedList<Course>> GetCoursesAsync(Guid authorId, CoursesResourceParameters resourceParameters)
	{
		ArgumentNullException.ThrowIfNull(resourceParameters);

		var courseCollection = _context.Courses as IQueryable<Course>;

		if (!string.IsNullOrWhiteSpace(resourceParameters.Title))
		{
			var title = resourceParameters.Title.Trim();
			courseCollection = courseCollection.Where(c =>
			c.Title == title);
		}
		if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
		{
			var searchQuery = resourceParameters.SearchQuery.Trim();
			courseCollection = courseCollection.Where(c =>
			c.Title.Contains(searchQuery) ||
			(c.Description != null && c.Description.Contains(searchQuery)));
		}

		return await PagedList<Course>.CreateAsync(courseCollection,
			resourceParameters.PageNumber
			, resourceParameters.PageSize);
	}

	public void UpdateCourse(Course course)
	{
		//no need for implementation it's just for clarification
	}


	public void AddAuthor(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		//// the repository fills the id (instead of using identity columns)
		//author.Id = Guid.NewGuid();

		foreach (var course in author.Courses)
		{
			course.Id = Guid.NewGuid();
		}

		_context.Authors.Add(author);
	}

	public async Task<bool> AuthorExistsAsync(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		return await _context.Authors.AnyAsync(a => a.Id == authorId);
	}

	public void DeleteAuthor(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		_context.Authors.Remove(author);
	}

	public async Task<Author?> GetAuthorAsync(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
	}
	public async Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters resourceParameters)
	{
		ArgumentNullException.ThrowIfNull(resourceParameters);

		var authorsCollection = _context.Authors as IQueryable<Author>;
		
		if(!string.IsNullOrWhiteSpace(resourceParameters.MainCategory))
		{
			var mainCategory = resourceParameters.MainCategory.Trim();
			authorsCollection = authorsCollection.Where(a=>
			a.MainCategory==mainCategory);
		}
		if(!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
		{
			var searchQuery = resourceParameters.SearchQuery.Trim();
			authorsCollection = authorsCollection.Where(a=>
			a.FirstName.Contains(searchQuery)||
			a.LastName.Contains(searchQuery)||
			a.MainCategory.Contains(searchQuery));
		}

		return await PagedList<Author>.CreateAsync(authorsCollection,
			resourceParameters.PageNumber
			,resourceParameters.PageSize);
	}

	public async Task<IEnumerable<Author>> GetAuthorsAsync()
	{
		return await _context.Authors.ToListAsync();
	}

	public async Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds)
	{
		ArgumentNullException.ThrowIfNull(authorIds);

		return await _context.Authors.Where(a => authorIds.Contains(a.Id))
			.OrderBy(a => a.FirstName)
			.OrderBy(a => a.LastName)
			.ToListAsync();
	}

	public void UpdateAuthor(Author author)
	{
		//no need for any implementation
	}

	public async Task<bool> SaveAsync()
	{
		return (await _context.SaveChangesAsync() >= 0);
	}

}
