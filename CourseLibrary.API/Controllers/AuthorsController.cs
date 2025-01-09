﻿using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
	private readonly ICourseLibraryRepository _courseLibraryRepository;
	private readonly IMapper _mapper;

	public AuthorsController(
		ICourseLibraryRepository courseLibraryRepository,
		IMapper mapper)
	{
		_courseLibraryRepository = courseLibraryRepository ??
			throw new ArgumentNullException(nameof(courseLibraryRepository));
		_mapper = mapper ??
			throw new ArgumentNullException(nameof(mapper));
	}

	[HttpGet]
	[HttpHead]
	public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
	{
		var authorsFromRepo = await _courseLibraryRepository
			.GetAuthorsAsync();

		return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
	}

	[HttpGet("{authorId}", Name = "GetAuthor")]
	public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
	{
		var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

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

		_courseLibraryRepository.AddAuthor(authorEntity);
		await _courseLibraryRepository.SaveAsync();

		var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

		return CreatedAtRoute("GetAuthor",
			new { authorId = authorToReturn.Id },
			authorToReturn);
	}

	[HttpPut("{authorId}")]
	public async Task<IActionResult> UpdateAuthor(Guid authorId,AuthorForUpdateDto author)
	{
		var authorEntity = await _courseLibraryRepository.GetAuthorAsync(authorId);
		
		if (authorEntity == null)
		{
			return NotFound();
			/*UnComment for Upserting
			authorEntity = _mapper.Map<Author>(author);
			authorEntity.Id = authorId;

			_courseLibraryRepository.AddAuthor(authorEntity);
			await _courseLibraryRepository.SaveAsync();

			var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
			return CreatedAtAction(nameof(GetAuthor),
				new {authorId},
				authorToReturn);*/
		}
		_mapper.Map(author,authorEntity);

		_courseLibraryRepository.UpdateAuthor(authorEntity);
		await _courseLibraryRepository.SaveAsync();
		return NoContent();
	}

	[HttpPatch("{authorId}")]
	public async Task<IActionResult> PartiallyUpdateAuthor(
		Guid authorId,
		JsonPatchDocument<AuthorForUpdateDto> patchDocument)
	{
		var authorEntity = await _courseLibraryRepository
			.GetAuthorAsync(authorId);
		
		if(authorEntity == null)
		{
			return NotFound();
			/* UnComment for Upserting
			var authorForUpdateDto = new AuthorForUpdateDto();
			patchDocument.ApplyTo(authorForUpdateDto,ModelState);

			if (!ModelState.IsValid)
				return BadRequest();

			var authorToAdd = _mapper.
				Map<Author>(authorForUpdateDto);
			authorToAdd.Id = authorId;

			_courseLibraryRepository.AddAuthor(authorToAdd);
			await _courseLibraryRepository.SaveAsync();

			var authorToReturn = _mapper.Map<AuthorDto>(authorToAdd);

			return CreatedAtAction(nameof(GetAuthor), 
				new {authorId},
				authorToReturn);*/
		}
		var authorToPatch = _mapper
			.Map<AuthorForUpdateDto>(authorEntity);
		patchDocument.ApplyTo(authorToPatch,ModelState);

		if (!ModelState.IsValid)
			return BadRequest();

		_mapper.Map(authorToPatch,authorEntity);
		_courseLibraryRepository.UpdateAuthor(authorEntity);
		await _courseLibraryRepository.SaveAsync();
		return NoContent();
	}

	[HttpDelete("{authorId}")]
	public async Task<IActionResult> DeleteAuthor(Guid authorId)
	{
		var authorToDelete = await _courseLibraryRepository.GetAuthorAsync(authorId);
		
		if (authorToDelete == null)
			return NotFound();

		_courseLibraryRepository.DeleteAuthor(authorToDelete);
		await _courseLibraryRepository.SaveAsync();
		
		return NoContent();
	}

	[HttpOptions]
	public IActionResult GetAuthorsOptions()
	{
		Response.Headers.Append("Allow", "GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS");
		return Ok();
	}
}