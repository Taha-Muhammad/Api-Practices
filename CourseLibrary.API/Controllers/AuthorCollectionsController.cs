using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
	[Route("api/authorcollections")]
	[ApiController]
	public class AuthorCollectionsController : ControllerBase
	{
		private readonly ICourseLibraryRepository _courseLibraryRepository;
		private readonly IMapper _mapper;
		public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
		{
			_courseLibraryRepository = courseLibraryRepository ??
				throw new ArgumentNullException(nameof(courseLibraryRepository));
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
		}
		
		[HttpHead("({authorIds})")]
		[HttpGet("({authorIds})")]
		public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorCollection
			([ModelBinder(BinderType = typeof(ArrayModelBinder))]
			[FromRoute] IEnumerable<Guid> authorIds)
		{
			var authorEntities = await _courseLibraryRepository
				.GetAuthorsAsync(authorIds);

			if(authorEntities.Count()!=authorIds.Count())
				return NotFound();

			var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
			return Ok(authorsToReturn);
		}

		[HttpPost]
		public async Task<ActionResult<IEnumerable<AuthorDto>>> CreateAuthorCollection(
			IEnumerable<AuthorForCreationDto> authorCollection)
		{
			var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);
			foreach (var author in authorEntities)
			{
				_courseLibraryRepository.AddAuthor(author);
			}
			await _courseLibraryRepository.SaveAsync();
			
			var authorCollectionToReturn =
				_mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
			
			var authorIdsAsString = string.Join(", ", 
				authorCollectionToReturn.Select(a=>a.Id));

			return CreatedAtAction(nameof(GetAuthorCollection), 
				new {authorIds= authorIdsAsString}
				,authorCollectionToReturn);
		}

		[HttpDelete("({authorIds})")]
		public async Task<IActionResult> DeleteAuthorCollection(
			[ModelBinder(BinderType = typeof(ArrayModelBinder))]
			[FromRoute] IEnumerable<Guid> authorIds)
		{
			var authors = await _courseLibraryRepository.GetAuthorsAsync(authorIds);
			if(authors == null||authors.Count()!=authorIds.Count())
				return NotFound();

			foreach (var author in authors)
			{
				_courseLibraryRepository.DeleteAuthor(author);
			}
			await _courseLibraryRepository.SaveAsync();

			return NoContent();
		}

		[HttpOptions]
		public IActionResult GetCoursesOptions()
		{
			Response.Headers.Append("Allow", "GET,HEAD,POST,DELETE,OPTIONS");
			return Ok();
		}
	}
}
