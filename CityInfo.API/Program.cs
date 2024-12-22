using CityInfo.API.DbContexts;
using CityInfo.API.Profiles;
using CityInfo.API.Services;
using CityInfo.API.Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(
	opt => {
		opt.ReturnHttpNotAcceptable = true;
	})
	.AddNewtonsoftJson()
	.AddJsonOptions
	(opt=>opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
	.AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CityInfoContext>();

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(typeof(CityProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
