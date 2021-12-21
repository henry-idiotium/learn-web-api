using WepA.Models.Dtos;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using WepA.Helpers.Messages;
using WepA.Helpers;
using System.Threading.Tasks;
using System.Text;
using System.Security.Claims;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using WepA.Interfaces.Repositories;

namespace WepA.Services
{
	public class AccountService : IAccountService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger _logger;
		private readonly IUserService _userService;
		private readonly IEmailService _emailService;
		private readonly IJwtService _jwtService;

		public AccountService(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			IEmailService emailService,
			IJwtService jwtService,
			IUserService userService,
			ILogger<AccountService> logger)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_emailService = emailService;
			_jwtService = jwtService;
			_userService = userService;
			_logger = logger;
		}

		public async Task<AuthenticateResponse> LoginAsync(LoginRequest account)
		{
			var result = await _signInManager.PasswordSignInAsync(account.Email, account.Password,
				isPersistent: false,
				lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.Unauthorized,
					message: ErrorResponseMessages.FailedLogin);
			}
			var user = await _userManager.FindByEmailAsync(account.Email);
			if (!await _userManager.IsEmailConfirmedAsync(user))
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.EmailNotVerify);
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.UserName),
			};
			var accessToken = _jwtService.GenerateAccessToken(claims);
			var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
			await _userService.AddRefreshTokenAsync(user, refreshToken);

			return new AuthenticateResponse(accessToken, refreshToken.Token);
		}

		public async Task RegisterAsync(ApplicationUser user, string password)
		{
			var userExists = _userService.ValidateUserExistence(user);
			if (userExists)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.UserAlreadyExists);
			}

			await _userManager.CreateAsync(user, password);
			var newUser = await _userManager.FindByEmailAsync(user.Email);
			if (newUser == null)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.InvalidRequest);
			}
			var addRole = await _userManager.AddToRoleAsync(newUser, "customer");
			if (!addRole.Succeeded)
			{
				_logger.LogError($"Failed to add role to user {newUser.Email}", addRole.Errors);
				throw new HttpStatusException(HttpStatusCode.InternalServerError,
					ErrorResponseMessages.ServerError);
			}
			var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
			var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));
			await _emailService.SendConfirmEmailAsync(newUser, code);

			var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
			await _userService.AddRefreshTokenAsync(newUser, refreshToken);
		}

		public async Task ConfirmEmailAsync(string userId, string code)
		{
			if (userId == null || code == null)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.InvalidRequest);
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.NotFoundUser);
			}

			var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded)
			{
				throw new HttpStatusException(
					status: HttpStatusCode.BadRequest,
					message: ErrorResponseMessages.FailedVerifyEmail);
			}
		}
	}
}