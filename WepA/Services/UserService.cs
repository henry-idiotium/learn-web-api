using WepA.Services.Interfaces;
using WepA.Models;
using WepA.Data.Repositories.Interfaces;
using WepA.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace WepA.Services
{
	public class UserService : IUserService
	{
		private readonly ILogger<UserService> _logger;
		private readonly IUserRepository _userRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserService(
			ILogger<UserService> logger,
			IUserRepository userRepository,
			UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			_userRepository = userRepository;
			_userManager = userManager;
		}

		public IEnumerable<ApplicationUser> GetUsers() => _userManager.Users;

		public async Task<ResultStateToken> CreateUserAsync(ApplicationUser user, string password)
		{
			var result = await _userManager.CreateAsync(user, password);
			var errorList = new List<string>();

			if (result.Succeeded)
			{
				await _userRepository.SaveAsync();
				_logger.LogInformation($"User '{user.Email}' created a new account with password!");
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

		public async Task<ApplicationUser> GetUserByIdAsync(string id) =>
			await _userManager.FindByIdAsync(id);

		public Task<ResultStateToken> DeleteUserByIdAsync(string id)
		{
			throw new System.NotImplementedException();
		}

		public Task<ResultStateToken> UpdateUserAsync(string id)
		{
			throw new System.NotImplementedException();
		}
	}
}