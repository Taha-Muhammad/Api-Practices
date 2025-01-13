using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services.PropertiesMappingDictionaries;
using CourseLibrary.API.Services.Repositories.Interfaces;
using CourseLibrary.API.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt => opt.ReturnHttpNotAcceptable = true)
	.AddNewtonsoftJson(setupAction =>
	setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
	)
	.AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction =>
	{
		setupAction.InvalidModelStateResponseFactory = context =>
		{
			// create a validation problem details object
			var problemDetailsFactory = context.HttpContext.RequestServices
				.GetRequiredService<ProblemDetailsFactory>();

			var validationProblemDetails = problemDetailsFactory
				.CreateValidationProblemDetails(
					context.HttpContext,
					context.ModelState);

			// add additional info not added by default
			validationProblemDetails.Detail =
				"See the errors field for details.";
			validationProblemDetails.Instance =
				context.HttpContext.Request.Path;

			// report invalid model state responses as validation issues
			validationProblemDetails.Type =
				"";
			validationProblemDetails.Status =
				StatusCodes.Status422UnprocessableEntity;
			validationProblemDetails.Title =
				"One or more validation errors occurred.";

			return new UnprocessableEntityObjectResult(
				validationProblemDetails)
			{
				ContentTypes = { "application/problem+json" }
			};
		};
	});

builder.Services.AddTransient<PropertyMapping<Course>, CoursePropertyMapping>();
builder.Services.AddTransient<PropertyMapping<Author>, AuthorPropertyMapping>();

builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddDbContext<CourseLibraryDbContext>(options =>
{
	options.UseSqlite(
		builder.Configuration
		.GetConnectionString("ApplicationConnectionString"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
	//app.MapScalarApiReference(opt=>
	//{
	//	opt.Title = "Course Library";
	//	opt.WithTheme(ScalarTheme.BluePlanet);
	//	opt.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
	//});
}
else
{
	app.UseExceptionHandler(appBuilder =>
	{
		appBuilder.Run(async context =>
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync(
				"An unexpected fault happened. Try again later.");
		});
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.ResetDatabaseAsync();

app.Run();

public static class DatabaseReset
{
	public static async Task ResetDatabaseAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			try
			{
				var context = scope.ServiceProvider.GetService<CourseLibraryDbContext>();
				if (context != null)
				{
					await context.Database.EnsureDeletedAsync();
					await context.Database.MigrateAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
				logger.LogError(ex, "An error occurred while migrating the database.");
			}
		}
	}
}