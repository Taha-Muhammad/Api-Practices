
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt=>opt.ReturnHttpNotAcceptable = true)
	.AddNewtonsoftJson(setupAction=>
	setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
	)
	.AddXmlDataContractSerializerFormatters();

builder.Services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
builder.Services.AddDbContext<CourseLibraryDbContext>(options =>
{
    options.UseSqlite(@"Data Source=library.db");
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