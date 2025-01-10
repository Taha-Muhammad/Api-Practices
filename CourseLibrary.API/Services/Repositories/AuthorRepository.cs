using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.Repositories;

public class AuthorRepository : IAuthorRepository
{
	private readonly CourseLibraryDbContext _context;
	private readonly Dictionary<string, PropertyMappingValue<Author>> _authorMappingDictionary;
	public AuthorRepository(CourseLibraryDbContext context
		, Dictionary<string, PropertyMappingValue<Author>> authorMappingDictionary)

	{
		_context = context;
		_authorMappingDictionary = authorMappingDictionary;
	}
	public async Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters resourceParameters)
	{
		ArgumentNullException.ThrowIfNull(resourceParameters);

		var authorsCollection = _context.Authors as IQueryable<Author>;

		if (!string.IsNullOrWhiteSpace(resourceParameters.MainCategory))
		{
			var mainCategory = resourceParameters.MainCategory.Trim();
			authorsCollection = authorsCollection.Where(a =>
			a.MainCategory == mainCategory);
		}
		if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
		{
			var searchQuery = resourceParameters.SearchQuery.Trim();
			authorsCollection = authorsCollection.Where(a =>
			a.FirstName.Contains(searchQuery) ||
			a.LastName.Contains(searchQuery) ||
			a.MainCategory.Contains(searchQuery));
		}
		if (!string.IsNullOrWhiteSpace(resourceParameters.OrderBy))
		{

			authorsCollection = authorsCollection.ApplySort
				(resourceParameters.OrderBy, _authorMappingDictionary);
		}

		return await PagedList<Author>.CreateAsync(authorsCollection,
			resourceParameters.PageNumber
			, resourceParameters.PageSize);
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
	public async Task<Author?> GetAuthorAsync(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentNullException(nameof(authorId));
		}

		return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
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
	public void UpdateAuthor(Author author)
	{
		//no need for any implementation
	}
	public void DeleteAuthor(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		_context.Authors.Remove(author);
	}
	public async Task<bool> SaveAsync()
	{
		return await _context.SaveChangesAsync() >= 0;
	}
}
