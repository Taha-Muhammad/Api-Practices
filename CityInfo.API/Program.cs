using CityInfo.API.DbContexts;
using CityInfo.API.Profiles;
using CityInfo.API.Services;
using CityInfo.API.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddSwaggerGen(setupAction => {
	setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new()
	{
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "Bearer",
		Description = "Input a valid token to access this API."
	});
	setupAction.AddSecurityRequirement(new()
	{
		{ 
		new()
		{
			Reference= new()
			{
				Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
				Id="CityInfoApiBearerAuth"
			}
		
		},
	new List<string>()}
	});
});

builder.Services.AddDbContext<CityInfoContext>();

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(typeof(CityProfile).Assembly);

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer(opt =>
	{	
		opt.TokenValidationParameters = new() 
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Authentication:Issuer"],
			ValidAudience = builder.Configuration["Authentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]!))

		};
	});
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
