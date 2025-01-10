using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IMapper _mapper;

	public AuthorsController(
		IAuthorRepository authorRepository,
		IMapper mapper)
	{
		_authorRepository = authorRepository ??
			throw new ArgumentNullException(nameof(authorRepository));
		_mapper = mapper ??
			throw new ArgumentNullException(nameof(mapper));
	}

	[HttpGet(Name = "GetAuthors")]
	[HttpHead]
	public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(
		[FromQuery] AuthorsResourceParameters resourceParameters)
	{
		var authorsFromRepo = await _authorRepository
			.GetAuthorsAsync(resourceParameters);
		Response.Headers.Append("X-Pagination",
		authorsFromRepo.CreatePaginationHeaderContent(resourceParameters, authorsFromRepo, Url, "GetAuthors", resourceParameters.MainCategory));

		return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
	}

	[HttpGet("{authorId}", Name = "GetAuthor")]
	public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
	{
		var authorFromRepo = await _authorRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}

		return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
	}

	[HttpPost]
	public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
	{

		var authorEntity = _mapper.Map<Entities.Author>(author);

		_authorRepository.AddAuthor(authorEntity);
		await _authorRepository.SaveAsync();

		var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

		return CreatedAtRoute("GetAuthor",
			new { authorId = authorToReturn.Id },
			authorToReturn);
	}

	[HttpPut("{authorId}")]
	public async Task<IActionResult> UpdateAuthor(Guid authorId, AuthorForUpdateDto author)
	{
		var authorEntity = await _authorRepository.GetAuthorAsync(authorId);

		if (authorEntity == null)
		{
			return NotFound();
			/*UnComment for Upserting
			authorEntity = _mapper.Map<Author>(author);
			authorEntity.Id = authorId;

			_authorRepository.AddAuthor(authorEntity);
			await _authorRepository.SaveAsync();

			var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
			return CreatedAtAction(nameof(GetAuthor),
				new {authorId},
				authorToReturn);*/
		}
		_mapper.Map(author, authorEntity);

		_authorRepository.UpdateAuthor(authorEntity);
		await _authorRepository.SaveAsync();
		return NoContent();
	}

	[HttpPatch("{authorId}")]
	public async Task<IActionResult> PartiallyUpdateAuthor(
		Guid authorId,
		JsonPatchDocument<AuthorForUpdateDto> patchDocument)
	{
		var authorEntity = await _authorRepository
			.GetAuthorAsync(authorId);

		if (authorEntity == null)
		{
			return NotFound();
			/* UnComment for Upserting
			var authorForUpdateDto = new AuthorForUpdateDto();
			patchDocument.ApplyTo(authorForUpdateDto,ModelState);

			if (!TryValidateModel(authorForUpdateDto))
				return ValidationProblem(ModelState);

			var authorToAdd = _mapper.
				Map<Author>(authorForUpdateDto);
			authorToAdd.Id = authorId;

			_authorRepository.AddAuthor(authorToAdd);
			await _authorRepository.SaveAsync();

			var authorToReturn = _mapper.Map<AuthorDto>(authorToAdd);

			return CreatedAtAction(nameof(GetAuthor), 
				new {authorId},
				authorToReturn);*/
		}
		var authorToPatch = _mapper
			.Map<AuthorForUpdateDto>(authorEntity);
		patchDocument.ApplyTo(authorToPatch, ModelState);

		if (!TryValidateModel(authorToPatch))
			return ValidationProblem(ModelState);

		_mapper.Map(authorToPatch, authorEntity);
		_authorRepository.UpdateAuthor(authorEntity);
		await _authorRepository.SaveAsync();
		return NoContent();
	}

	[HttpDelete("{authorId}")]
	public async Task<IActionResult> DeleteAuthor(Guid authorId)
	{
		var authorToDelete = await _authorRepository.GetAuthorAsync(authorId);

		if (authorToDelete == null)
			return NotFound();

		_authorRepository.DeleteAuthor(authorToDelete);
		await _authorRepository.SaveAsync();

		return NoContent();
	}

	[HttpOptions]
	public IActionResult GetAuthorsOptions()
	{
		Response.Headers.Append("Allow", "GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS");
		return Ok();
	}
	public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
	{
		var options = HttpContext.RequestServices
			.GetRequiredService<IOptions<ApiBehaviorOptions>>();
		return (ActionResult)options.Value
			.InvalidModelStateResponseFactory(ControllerContext);
	}
}