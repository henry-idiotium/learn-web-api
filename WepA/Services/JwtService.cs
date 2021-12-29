using WepA.Models.Dtos;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using WepA.Helpers.Settings;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using WepA.Helpers;
using System.Net;
using WepA.Helpers.Messages;

namespace WepA.Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtSettings _jwtSettings;
		private readonly byte[] _jwtSecret;
		private readonly List<string> _validLocations = new()
		{
			"https://localhost:5001",
			"https://localhost:5000"
		};
		public JwtService(IOptions<JwtSettings> jwtSettings)
		{
			_jwtSettings = jwtSettings.Value;
			_jwtSecret = EncryptHelpers.EncodeASCII(_jwtSettings.Secret);
		}

		public AccessToken GenerateAccessToken(IEnumerable<Claim> claims)
		{
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _jwtSettings.Issuer,
				Audience = _jwtSettings.Audience,
				Subject = new ClaimsIdentity(claims),
				// Expires = DateTime.UtcNow.AddDays(_jwtSettings.AccessTokenExpiration),
				Expires = DateTime.UtcNow.AddSeconds(5), // Testing
				SigningCredentials = new SigningCredentials(
					key: new SymmetricSecurityKey(_jwtSecret),
					algorithm: SecurityAlgorithms.HmacSha256Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			// var expireIn = (int)new TimeSpan(_jwtSettings.AccessTokenExpiration,0,0,0).TotalSeconds;
			var expireIn = (int)new TimeSpan(0,0,0,5).TotalSeconds; // Testing
			return new AccessToken(tokenHandler.WriteToken(token), expireIn);
		}

		public RefreshToken GenerateRefreshToken(string userId)
		{
			if (userId == null)
			{
				throw new HttpStatusException(HttpStatusCode.BadRequest,
											  ErrorResponseMessages.InvalidRequest);
			}
			using var cryptoProvider = new RNGCryptoServiceProvider();
			var randomBytes = new byte[64];
			cryptoProvider.GetBytes(randomBytes);
			var refreshToken = new RefreshToken
			{
				Token = Convert.ToBase64String(randomBytes),
				Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiration),
				Created = DateTime.UtcNow,
				UserId = userId
			};
			return refreshToken;
		}

		public ClaimsPrincipal GetClaimsPrincipal(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtToken = tokenHandler.ReadToken(token);
				if (jwtToken is null) return null;

				var validationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = false,

					ValidIssuers = _validLocations,
					ValidAudiences = _validLocations,
					IssuerSigningKey = new SymmetricSecurityKey(_jwtSecret)
				};

				var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
				return principal;
			}
			catch
			{
				return null;
			}
		}

		public string Validate(string token)
		{
			if (token is null) return null;
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,

					ValidIssuers = _validLocations,
					ValidAudiences = _validLocations,
					IssuerSigningKey = new SymmetricSecurityKey(_jwtSecret),
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var jwtToken = validatedToken as JwtSecurityToken;
				var userId = jwtToken.Claims
					.FirstOrDefault(claim => claim.Type == "nameid" && claim.Value != null)
					.Value;
				return userId; // Upon successful validation return user id from JWT token
			}
			catch
			{
				return null; // When fails validation
			}
		}
	}
}