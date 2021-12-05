using WepA.Interfaces.Utils;
using WepA.Extensions.Helpers;
using System.Text;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using WepA.Interfaces.Services;
using System.Threading.Tasks;

namespace WepA.Utils
{
	public class JwtUtil : IJwtUtil
	{
		private readonly JwtSettings _jwtSettings;
		private readonly IUserService _userService;

		public JwtUtil(
			IUserService userService,
			IOptions<JwtSettings> jwtSettings)
		{
			_userService = userService;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<string> GenerateTokenAsync(string email, IList<string> roles)
		{
			// Init claims
			var user = await _userService.GetUserByEmailAsync(email);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.UserName)
			};
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			// Generate token with valid duration of 7 days  
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = "https://localhost:5001",
				Audience = "https://localhost:5001",
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public string Validate(string token)
		{
			if (token is null) return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

			try
			{
				var validLocations = new List<string>{
					"https://localhost:5001",
					"https://localhost:5000"
				};
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,

					ValidIssuers = validLocations,
					ValidAudiences = validLocations,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				}, out SecurityToken validatedToken);

				var jwtToken = validatedToken as JwtSecurityToken;
				var userId = jwtToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
				return userId; // Upon successful validation return user id from JWT token
			}
			catch
			{
				return null; // When fails validation
			}
		}

		public ClaimsPrincipal GetPrincipal(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();

				var jwtToken = tokenHandler.ReadToken(token);
				if (jwtToken is null) return null;

				var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

				var validLocations = new List<string>{
					"https://localhost:5001",
					"https://localhost:5000"
				};
				var validationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,

					ValidIssuers = validLocations,
					ValidAudiences = validLocations,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};

				var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}