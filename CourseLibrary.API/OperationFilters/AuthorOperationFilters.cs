using CourseLibrary.API.Models;
using CourseLibrary.API.Models.AuthorDtos;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CourseLibrary.API.OperationFilters
{
	public class AuthorOperationFilters : IOperationFilter
	{
		public void Apply(OpenApiOperation operation,
			OperationFilterContext context)
		{
			if (operation.OperationId != "GetAuthors" &&
				operation.OperationId != "GetAuthor" &&
				operation.OperationId != "CreateAuthor")
			{
				return;
			}
			if (operation.OperationId == "GetAuthors")
			{
				if (operation.Responses.Any(res =>
				res.Key == StatusCodes.Status200OK.ToString()))
				{
					var schema = typeof(IEnumerable<AuthorWithLinks>);
					operation.Responses[StatusCodes.Status200OK.ToString()]
					.Content.Add(
					"application/vnd.companyname.hateoas+json",
					new OpenApiMediaType()
					{
						Schema = context.SchemaGenerator
					.GenerateSchema(schema,
					context.SchemaRepository)
					});
				}
			}
			if (operation.OperationId == "GetAuthor")
			{
				if (operation.Responses.Any(res=>
				res.Key == StatusCodes.Status200OK.ToString()))
				{
					operation.Responses[StatusCodes.Status200OK.ToString()]
						.Content.Add(
						"application/vnd.companyname.hateoas+json",
						new OpenApiMediaType()
						{
							Schema = context.SchemaGenerator
						.GenerateSchema(typeof(AuthorWithLinks),
						context.SchemaRepository)
						});
					operation.Responses[StatusCodes.Status200OK.ToString()]
						.Content.Add(
						"application/vnd.companyname.author.friendly.hateoas+json",
						new OpenApiMediaType()
						{
							Schema = context.SchemaGenerator
						.GenerateSchema(typeof(AuthorWithLinks),
						context.SchemaRepository)
						});
					operation.Responses[StatusCodes.Status200OK.ToString()]
						.Content.Add(
						"application/vnd.companyname.author.full+json",
						new OpenApiMediaType()
						{
							Schema = context.SchemaGenerator
						.GenerateSchema(typeof(FullAuthorDto),
						context.SchemaRepository)
						});
					operation.Responses[StatusCodes.Status200OK.ToString()]
						.Content.Add(
						"application/vnd.companyname.author.full.hateoas+json",
						new OpenApiMediaType()
						{
							Schema = context.SchemaGenerator
						.GenerateSchema(typeof(FullAuthorWithLinks),
						context.SchemaRepository)
						});
				} 
			}
			if (operation.OperationId == "CreateAuthor")
			{
				operation.RequestBody
					.Content.Add(
					"application/vnd.companyname.authorforcreationwithdateofdeath+json",
					new OpenApiMediaType() { Schema =context.SchemaGenerator
					.GenerateSchema(typeof(AuthorWithDateOfDeathForCreationDto),
					context.SchemaRepository)});
			}

		}
	}
	class FullAuthorWithLinks : FullAuthorDto
	{
		public IEnumerable<LinkDto> Links { get; set; } = null!;
	}
	class AuthorWithLinks : AuthorDto
	{
		public IEnumerable<LinkDto> Links { get; } = null!;
	}
}
