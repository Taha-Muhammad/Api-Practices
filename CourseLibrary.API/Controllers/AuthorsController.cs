using AutoMapper;
using CourseLibrary.API.ActionConstraints;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Models.AuthorDtos;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using CourseLibrary.API.Services.PropertiesMappingDictionaries;
using CourseLibrary.API.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IMapper _mapper;
	private readonly PropertyMapping<Author> _propertyMapping;
	private readonly IPropertyCheckerService _propertyCheckerService;
	private readonly ProblemDetailsFactory _problemDetailsFactory;

	public AuthorsController(
		IAuthorRepository authorRepository,
		IMapper mapper,
		PropertyMapping<Author> propertyMapping,
		IPropertyCheckerService propertyCheckerService,
		ProblemDetailsFactory problemDetailsFactory)
	{
		_authorRepository = authorRepository ??
			throw new ArgumentNullException(nameof(authorRepository));
		_mapper = mapper ??
			throw new ArgumentNullException(nameof(mapper));
		_propertyMapping = propertyMapping ??
			throw new ArgumentNullException(nameof(propertyMapping));
		_propertyCheckerService = propertyCheckerService ??
			throw new ArgumentNullException(nameof(propertyCheckerService));
		_problemDetailsFactory = problemDetailsFactory ??
			throw new ArgumentNullException(nameof(problemDetailsFactory));
	}

	[RequestHeaderMatchesMediaType("Accept",
		"application/json")]
	[Produces("application/json", Type = typeof(AuthorDto))]
	[HttpGet(Name = "GetAuthors")]
	[HttpHead]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetAuthors(
		[FromQuery] AuthorsResourceParameters resourceParameters)
	{
		if (!_propertyMapping.ValidMappingExistsFor(resourceParameters.OrderBy))
			return BadRequest();

		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>
			(resourceParameters.Fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {resourceParameters.Fields}")
				);
		}

		var authorsFromRepo = await _authorRepository
			.GetAuthorsAsync(resourceParameters);

		Response.Headers.Append("X-Pagination",
		authorsFromRepo.CreatePaginationHeaderContent(resourceParameters,
		authorsFromRepo, Url, "GetAuthors"
		, resourceParameters.MainCategory, null));

		return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo)
			.ShapeData(resourceParameters.Fields));
	}

	[RequestHeaderMatchesMediaType("Accept",
		"application/vnd.companyname.hateoas+json")]
	[Produces("application/vnd.companyname.hateoas+json")]
	[HttpGet]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<IActionResult> GetAuthorsWithLinks(
		[FromQuery] AuthorsResourceParameters resourceParameters)
	{
		if (!_propertyMapping.ValidMappingExistsFor(resourceParameters.OrderBy))
			return BadRequest();

		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>
			(resourceParameters.Fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {resourceParameters.Fields}")
				);
		}

		var authorsFromRepo = await _authorRepository
			.GetAuthorsAsync(resourceParameters);

		Response.Headers.Append("X-Pagination",
		authorsFromRepo.CreatePaginationHeaderContent(resourceParameters,
		authorsFromRepo, Url, "GetAuthors"
		, resourceParameters.MainCategory, null, true));

		var links = CreateLinksForAuthors(resourceParameters,
			authorsFromRepo.HasNext, authorsFromRepo.HasPrevious);
		var authorsBeforeShaping = _mapper.Map<List<AuthorDto>>(authorsFromRepo);
		var authorIdsDictionary = new Dictionary<string, Guid>();
		for (int i = 0; i < authorsBeforeShaping.Count; i++)
		{
			authorIdsDictionary.Add("Id" + i, authorsBeforeShaping[i].Id);
		}
		var shapedAuthors = (IEnumerable<AuthorDto>)authorsBeforeShaping;
		var shapedAuthors2 = shapedAuthors.ShapeData(resourceParameters.Fields);
		int j = 0;
		var shapedAuthorsWithLinks = shapedAuthors2
			.Select(author =>
			{
				var authorAsDictionary = author as IDictionary<string, object?>;
				var authorLinks = CreateLinksForAuthor(
					(Guid)authorIdsDictionary["Id" + j]!, null);
				authorAsDictionary.Add("links", authorLinks);
				j++;
				return authorAsDictionary;
			});
		var linkedCollectionResource = new
		{
			value = shapedAuthorsWithLinks,
			links
		};

		return Ok(linkedCollectionResource);
	}

	[RequestHeaderMatchesMediaType("Accept",
		"application/json",
		 "application/vnd.companyname.author.friendly+json")]
	[Produces("application/json",
	"application/vnd.companyname.author.friendly+json", Type = typeof(AuthorDto))]
	[HttpGet("{authorId}", Name = "GetAuthor")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetAuthor(Guid authorId, string? fields)
	{
		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {fields}")
				);
		}
		var authorFromRepo = await _authorRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}

		//if you want to allow requesting dto with
		//missing fields without hateoas links keep it as.
		//if not than remove ShapeData.
		return Ok(_mapper.Map<AuthorDto>(authorFromRepo)
				.ShapeData(fields));
	}

	//hateoas is placed in the (media type name) place
	[RequestHeaderMatchesMediaType("Accept",
		"application/vnd.companyname.hateoas+json",
"application/vnd.companyname.author.friendly.hateoas+json")]
	[Produces("application/vnd.companyname.hateoas+json",
"application/vnd.companyname.author.friendly.hateoas+json")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet("{authorId}")]
	public async Task<IActionResult> GetAuthorWithLinks(Guid authorId, string? fields)
	{
		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {fields}")
				);
		}
		var authorFromRepo = await _authorRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}

		IEnumerable<LinkDto> links = CreateLinksForAuthor(authorId, fields);

		var friendlyResourceToReturn = _mapper.Map<AuthorDto>(authorFromRepo)
		.ShapeData(fields) as IDictionary<string, object?>;

		friendlyResourceToReturn.Add("links", links);
		return Ok(friendlyResourceToReturn);
	}

	[RequestHeaderMatchesMediaType("Accept",
		"application/vnd.companyname.author.full+json")]
	[Produces("application/vnd.companyname.author.full+json")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet("{authorId}")]
	public async Task<IActionResult> GetFullAuthor(Guid authorId, string? fields)
	{
		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {fields}")
				);
		}
		var authorFromRepo = await _authorRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}
		var authorToReturn = _mapper.Map<FullAuthorDto>(authorFromRepo)
				.ShapeData(fields);
		return Ok(authorToReturn);
	}

	[RequestHeaderMatchesMediaType("Accept",
		"application/vnd.companyname.author.full.hateoas+json")]
	[Produces("application/vnd.companyname.author.full.hateoas+json")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet("{authorId}")]
	public async Task<IActionResult> GetFullAuthorWithLinks(Guid authorId, string? fields)
	{
		if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
		{
			return BadRequest(_problemDetailsFactory
				.CreateProblemDetails(HttpContext,
				statusCode: 400,
				detail: $"Not all requested data shaping fields exist on " +
				$"the resource: {fields}")
				);
		}
		var authorFromRepo = await _authorRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}
		var fullResourceToReturn = _mapper.Map<FullAuthorDto>
				(authorFromRepo)
				.ShapeData(fields) as IDictionary<string, object?>;
		var links = CreateLinksForAuthor(authorId, fields);
		fullResourceToReturn.Add("links", links);
		return Ok(fullResourceToReturn);
	}

	[RequestHeaderMatchesMediaType("Content-Type",
		"application/json",
		"application/vnd.companyname.authorforcreation+json")]
	[Consumes("application/json",
		"application/vnd.companyname.authorforcreation+json")]
	[HttpPost(Name = "CreateAuthor")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
	{
		var authorEntity = _mapper.Map<Author>(author);

		_authorRepository.AddAuthor(authorEntity);
		await _authorRepository.SaveAsync();

		var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

		var links = CreateLinksForAuthor(authorToReturn.Id, null);

		var linkedResourceToReturn = authorToReturn
			.ShapeData(null) as IDictionary<string, object?>;
		linkedResourceToReturn.Add("links", links);
		return CreatedAtRoute("GetAuthor",
			new { authorId = linkedResourceToReturn["Id"] },
			linkedResourceToReturn);
	}

	[RequestHeaderMatchesMediaType("Content-Type",
		"application/vnd.companyname.authorforcreationwithdateofdeath+json")]
	[Consumes("application/vnd.companyname.authorforcreationwithdateofdeath+json")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpPost]
	public async Task<ActionResult<AuthorDto>> CreateAuthorWithDateOfDeath(AuthorWithDateOfDeathForCreationDto author)
	{

		var authorEntity = _mapper.Map<Author>(author);

		_authorRepository.AddAuthor(authorEntity);
		await _authorRepository.SaveAsync();

		var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

		var links = CreateLinksForAuthor(authorToReturn.Id, null);

		var linkedResourceToReturn = authorToReturn
			.ShapeData(null) as IDictionary<string, object?>;
		linkedResourceToReturn.Add("links", links);
		return CreatedAtRoute("GetAuthor",
			new { authorId = linkedResourceToReturn["Id"] },
			linkedResourceToReturn);
	}

	[HttpPut("{authorId}")]

	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	private IEnumerable<LinkDto> CreateLinksForAuthors(
		AuthorsResourceParameters authorsResourceParameters,
		bool hasNext, bool hasPrevious)
	{
		var links = new List<LinkDto>();
		links.Add(
			new(ResourceUris.CreateResourceUri(
				authorsResourceParameters,
				ResourceUris.ResourceUriType.Current,
				"GetAuthors",
				Url,
				authorsResourceParameters.MainCategory),
				"self", "GET"));
		if (hasNext)
		{
			links.Add(
				new(ResourceUris.CreateResourceUri(
					authorsResourceParameters,
					ResourceUris.ResourceUriType.NextPage,
					"GetAuthors",
					Url,
					authorsResourceParameters.MainCategory),
					"nextPage", "GET"));
		}
		if (hasPrevious)
		{
			links.Add(
	new(ResourceUris.CreateResourceUri(
		authorsResourceParameters,
		ResourceUris.ResourceUriType.PreviousPage,
		"GetAuthors",
		Url,
		authorsResourceParameters.MainCategory),
		"previousPage", "GET"));
		}
		return links;
	}
	private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId,
		string? fields)
	{
		var links = new List<LinkDto>();

		if (string.IsNullOrWhiteSpace(fields))
		{
			links.Add(
				new(Url.Link("GetAuthor", new { authorId }),
				"self",
				"GET"));
		}
		else
		{
			links.Add(
				new(Url.Link("GetAuthor", new { authorId, fields }),
				"self",
				"GET"));
		}
		links.Add(
		new(Url.Link("CreateCourseForAuthor",
		new { authorId }),
		"create_course_for_author",
		"POST"));

		links.Add(
		new(Url.Link("GetCoursesForAuthor", new { authorId }),
		"courses",
		"GET"));

		return links;
	}
}