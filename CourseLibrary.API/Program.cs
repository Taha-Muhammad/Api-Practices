
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt=>opt.ReturnHttpNotAcceptable = true)
	.AddNewtonsoftJson(setupAction=>
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


builder.Services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
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