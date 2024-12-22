using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		const string UserName = "username";
		const string Password = "password";
		
		private readonly IConfiguration _configuration;
		public AuthenticationController(IConfiguration configuration)
        {
			_configuration = configuration;
		}

        public class AuthenticationRequestBody
		{
			public string? UserName { get; set; }
			public string? Password { get; set; }
		}
		[HttpPost("authenticate")]
		public ActionResult<string> Authenticate(AuthenticationRequestBody request)
		{
			var user = ValidateUserCredentials(request.UserName, request.Password);
			if (user == null) 
				return Unauthorized();
			var secret = _configuration["Authentication:SecretForKey"];
			var securityKey = new SymmetricSecurityKey(
				Convert.FromBase64String(secret!));
			var signingCredentials = new SigningCredentials
				(securityKey, SecurityAlgorithms.HmacSha256);

			var claimsForToken = new List<Claim>()
			{
				new Claim("sub",user.UserId.ToString()),
				new("given_name",user.FirstName),
				new("family_name",user.LastName),
				new("city",user.City)
			};
			var jwtSecurityToken = new JwtSecurityToken
				(_configuration["Authentication:Issuer"],
				_configuration["Authentication:Audience"],
				claimsForToken,
				DateTime.UtcNow,
				DateTime.UtcNow.AddHours(1),
				signingCredentials);
			var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
			return Ok(token);
		}

		private CityInfoUser? ValidateUserCredentials(string? userName, string? password)
		{
			if (userName == UserName && password == Password)
				return new CityInfoUser(1, userName, "Taha", "Mohamed", "Cairo");
			return null;
		}

		private class CityInfoUser
		{
			public int UserId { get; set; }
			public string UserName { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string City { get; set; }
			public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
			{
				UserId = userId;
				UserName = userName;
				FirstName = firstName;
				LastName = lastName;
				City = city;
			}
		}
	}
}
