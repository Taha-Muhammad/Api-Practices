using GloboTicket.TicketManagement.Application;
using GloboTicket.TicketManagement.Infrastructure;
using GloboTicket.TicketManagement.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices()
	.AddInfrastructureServices(builder.Configuration)
	.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(opt =>
	{
		opt.SwaggerEndpoint("/openapi/v1.json", "Globo Ticket");
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.ResetDatabaseAsync();
app.Run();

public static class WepApplicationExtensions
{
	public static async Task ResetDatabaseAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			try
			{
				var context = scope.ServiceProvider
					.GetService<GloboTicketDbContext>();
				if (context != null)
				{
					await context.Database.EnsureDeletedAsync();
					await context.Database.MigrateAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
				logger?.LogError(ex, "An error occurred while migrating the database.");
			}
		}
	}
}