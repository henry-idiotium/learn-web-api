using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WepA.Common;
using WepA.Data.Repositories.Interfaces;
using WepA.Models;
using WepA.Services.Interfaces;

namespace WepA.Services
{
	public class AccountService : IAccountService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AccountService> _logger;
		private readonly IUserRepository _userRepository;

		public AccountService(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<AccountService> logger,
			IUserRepository userRepository)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_userRepository = userRepository;
		}

		public async Task<ResultStateToken> LoginAccountAsync(Account input)
		{
			var result = await _signInManager.PasswordSignInAsync(
				input.Email,
				input.Password,
				input.RememberMe,
				lockoutOnFailure: false);
			var errorList = new List<string>();

			if (result.Succeeded)
			{
				_logger.LogInformation($"User {input.Email} logged in.");
			}
			else
			{
				errorList.Add("Invalid login attempt.");
			}

			return new(result.Succeeded, errorList.ToArray());
		}

		public async Task<ResultStateToken> RegisterAccountAsync(ApplicationUser input, string password)
		{
			var result = await _userManager.CreateAsync(input, password);
			var errorList = new List<string>();

			if (result.Succeeded)
			{
				await _userRepository.SaveAsync();
				_logger.LogInformation($"User '{input.Email}' created a new account with password!");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					errorList.Add(error.Description);
				}
			}

			return new(result.Succeeded, errorList.ToArray());
		}
	}
}