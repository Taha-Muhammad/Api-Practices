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
[Route("api/authors/{authorId}/courses")]
public class CoursesController : ControllerBase
{
	private readonly ICourseRepository _courseRepository;
	private readonly IMapper _mapper;

	public CoursesController(ICourseRepository courseRepository,
		IMapper mapper)
	{
		_courseRepository = courseRepository ??
			throw new ArgumentNullException(nameof(courseRepository));
		_mapper = mapper ??
			throw new ArgumentNullException(nameof(mapper));
	}

	[HttpGet(Name ="GetCoursesForAuthor")]
	[HttpHead]
	public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesForAuthor(Guid authorId, [FromQuery] CoursesResourceParameters coursesResourceParameters)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
		{
			return NotFound();
		}

		var coursesForAuthorFromRepo = await _courseRepository.GetCoursesAsync(authorId,coursesResourceParameters);
		Response.Headers.Append("X-Pagination",coursesForAuthorFromRepo
			.CreatePaginationHeaderContent(
				coursesResourceParameters
				, coursesForAuthorFromRepo, Url,
				"GetCoursesForAuthor", null,
				coursesResourceParameters.Title));
		return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
	}

	[HttpGet("{courseId}")]
	public async Task<ActionResult<CourseDto>> GetCourseForAuthor(Guid authorId, Guid courseId)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
		{
			return NotFound();
		}

		var courseForAuthorFromRepo = await _courseRepository.GetCourseAsync(authorId, courseId);

		if (courseForAuthorFromRepo == null)
		{
			return NotFound();
		}
		return Ok(_mapper.Map<CourseDto>(courseForAuthorFromRepo));
	}

	[HttpPost]
	public async Task<ActionResult<CourseDto>> CreateCourseForAuthor(
			Guid authorId, CourseForCreationDto course)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
		{
			return NotFound();
		}

		var courseEntity = _mapper.Map<Entities.Course>(course);
		_courseRepository.AddCourse(authorId, courseEntity);
		await _courseRepository.SaveAsync();

		var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
		return CreatedAtAction(nameof(GetCourseForAuthor),
			new { authorId, courseId = courseToReturn.Id },
			courseToReturn);
	}

	[HttpPut("{courseId}")]
	public async Task<IActionResult> UpdateCourseForAuthor(Guid authorId,
	  Guid courseId,
	  CourseForUpdateDto course)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
		{
			return NotFound();
		}

		var courseForAuthorFromRepo = await _courseRepository.
			GetCourseAsync(authorId, courseId);

		if (courseForAuthorFromRepo == null)
		{
			return NotFound();
			/*UnComment for Upserting			
 			var courseToAdd = _mapper.Map<Course>(course);
			courseToAdd.Id = courseId;

			_courseRepository.AddCourse(authorId, courseToAdd);
			await _courseRepository.SaveAsync();

			var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
			return CreatedAtAction(nameof(GetCourseForAuthor),
				new { authorId, courseId },
				courseToReturn);*/
		}

		_mapper.Map(course, courseForAuthorFromRepo);

		_courseRepository.UpdateCourse(courseForAuthorFromRepo);

		await _courseRepository.SaveAsync();
		return NoContent();
	}

	[HttpPatch("{courseId}")]
	public async Task<IActionResult> PartiallyUpdateCourseForAuthor(
		Guid authorId,
		Guid courseId,
		JsonPatchDocument<CourseForUpdateDto> patchDocument)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
			return NotFound();

		var courseForAuthorFromRepo = await _courseRepository
			.GetCourseAsync(authorId, courseId);
		
		if (courseForAuthorFromRepo == null)
		{
			return NotFound();
			/* UnComment for Upserting
			var courseDto = new CourseForUpdateDto();
			patchDocument.ApplyTo(courseDto,ModelState);

			if (!TryValidateModel(courseDto))
				return ValidationProblem(ModelState);
			
			var courseToAdd = _mapper.Map<Course>(courseDto);
			courseToAdd.Id = courseId;
			
			_courseRepository.AddCourse(authorId, courseToAdd);
			await _courseRepository.SaveAsync();

			var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
			
			return CreatedAtAction(nameof(GetCourseForAuthor), 
				new {authorId, courseId},
				courseToReturn);*/
		}

		var courseToPatch = _mapper
			.Map<CourseForUpdateDto>(courseForAuthorFromRepo);

		patchDocument.ApplyTo(courseToPatch,ModelState);

		if (!TryValidateModel(courseToPatch))
			return ValidationProblem(ModelState);

		_mapper.Map(courseToPatch, courseForAuthorFromRepo);
		_courseRepository.UpdateCourse(courseForAuthorFromRepo);
		await _courseRepository.SaveAsync();

		return NoContent();
	}

	[HttpDelete("{courseId}")]
	public async Task<ActionResult> DeleteCourseForAuthor(Guid authorId, Guid courseId)
	{
		if (!await _courseRepository.AuthorExistsAsync(authorId))
		{
			return NotFound();
		}

		var courseForAuthorFromRepo = await _courseRepository.GetCourseAsync(authorId, courseId);

		if (courseForAuthorFromRepo == null)
		{
			return NotFound();
		}

		_courseRepository.DeleteCourse(courseForAuthorFromRepo);
		await _courseRepository.SaveAsync();

		return NoContent();
	}

	[HttpOptions]
	public IActionResult GetCoursesOptions(Guid authorId)
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