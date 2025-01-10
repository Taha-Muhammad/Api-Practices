using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;

namespace CourseLibrary.API.Services.Repositories.Interfaces;

public interface IAuthorRepository
{
	Task<IEnumerable<Author>> GetAuthorsAsync();
	Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters resourceParameters);
	Task<Author?> GetAuthorAsync(Guid authorId);
	Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
	void AddAuthor(Author author);
	void DeleteAuthor(Author author);
	void UpdateAuthor(Author author);
	Task<bool> SaveAsync();
}
