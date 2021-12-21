using WepA.Models.Dtos;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using WepA.Interfaces.Repositories;
using WepA.Helpers.Messages;
using WepA.Helpers;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using WepA.Models.Dtos.Token;
using System.Security.Claims;

namespace WepA.Services
{
	public class UserService : IUserService
	{
		private readonly ILogger<UserService> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IJwtService _jwtService;
		private readonly IEmailService _emailService;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserService(
			UserManager<ApplicationUser> userManager,
			ILogger<UserService> logger,
			IEmailService emailService,
			IJwtService jwtService,
			IUserRepository userRepository)
		{
			_userManager = userManager;
			_logger = logger;
			_emailService = emailService;
			_jwtService = jwtService;
			_userRepository = userRepository;
		}

		public IEnumerable<ApplicationUser> GetAll() => _userManager.Users;
		public async Task<ApplicationUser> GetByIdAsync(string id) => await _userManager.FindByIdAsync(id);
		public async Task<ApplicationUser> GetByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);

		public async Task CreateAsync(ApplicationUser user, string password, List<string> roles)
		{
			var createUser = await _userManager.CreateAsync(user, password);
			if (!createUser.Succeeded)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.InvalidRequest);
			}
			var newUser = await _userManager.FindByEmailAsync(user.Email);
			var addRoles = await _userManager.AddToRolesAsync(newUser, roles);
			if (!addRoles.Succeeded)
			{
				_logger.LogError($"Failed to add role to user {user.Email}.", addRoles.Errors);
				throw new HttpStatusException(
					status: HttpStatusCode.InternalServerError,
					message: ErrorResponseMessages.ServerError);
			}
			var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
			var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));
			await _emailService.SendConfirmEmailAsync(newUser, code);
		}
		public Task DeleteAsync(string userId) => throw new NotImplementedException();
		public Task UpdateAsync(ApplicationUser user) => throw new NotImplementedException();

		public async Task AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken)
		{
			if (refreshToken == null || !refreshToken.IsActive)
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRefreshToken);

			await _userRepository.AddRefreshTokenAsync(user, refreshToken);
		}

		public async Task RevokeRefreshToken(string token)
		{
			var refreshToken = _userRepository.GetRefreshToken(token);
			if (!refreshToken.IsActive)
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRefreshToken);

			await _userRepository.RevokeRefreshTokenAsync(refreshToken, "Revoked without replacement");
		}

		public async Task<AuthenticateResponse> RotateTokensAsync(TokenRotateRequest model)
		{
			var user = _userRepository.GetByRefreshToken(model.RefreshToken);
			var refreshToken = _userRepository.GetRefreshToken(model.RefreshToken);
			if (refreshToken == null)
			{
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRequest);
			}
			if (refreshToken.IsRevoked)
			{
				// revoke all descendant tokens in case this token has been compromised
				await _userRepository.RevokeRefreshTokenDescendantsAsync(refreshToken, user,
					reason: $"Attempted reuse of revoked ancestor token: {model.RefreshToken}");
			}
			if (!refreshToken.IsActive)
			{
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRefreshToken);
			}

			// rotate token
			var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);
			await _userRepository.RevokeRefreshTokenAsync(
				token: refreshToken,
				reason: "Replaced by new token",
				replacedByToken: newRefreshToken.Token);
			await _userRepository.RemoveOutdatedRefreshTokensAsync(user);

			// Get principal from expired token
			var principal = _jwtService.GetClaimsPrincipal(model.AccessToken);
			if (principal == null)
			{
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRequest);
			}
			var accessToken = _jwtService.GenerateAccessToken(principal.Claims);
			return new AuthenticateResponse(accessToken, newRefreshToken.Token);
		}

		public bool ValidateUserExistence(ApplicationUser user)
		{
			if (user == null)
			{
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRequest);
			}
			var userExists = _userRepository.ValidateUserExistence(user);
			return userExists;
		}
	}
}