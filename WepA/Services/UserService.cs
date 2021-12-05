using WepA.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WepA.Interfaces.Services;
using System.Security.Claims;

namespace WepA.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public UserService(UserManager<ApplicationUser> userManager) => _userManager = userManager;

		public IEnumerable<ApplicationUser> GetUsers() => _userManager.Users;

		public async Task<bool> CreateUserAsync(ApplicationUser user,
			string password,
			List<string> roles)
		{
			var createUser = await _userManager.CreateAsync(user, password);
			if (!createUser.Succeeded) return false;

			var newUser = await _userManager.FindByEmailAsync(user.Email);
			// TODO - Add role validation
			await _userManager.AddToRolesAsync(newUser, roles);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, newUser.Id),
				new Claim(ClaimTypes.Email, newUser.Email),
				new Claim(ClaimTypes.Name, newUser.UserName)
			};
			foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));

			var addClaims = await _userManager.AddClaimsAsync(newUser, claims);

			if (!(createUser.Succeeded && addClaims.Succeeded))
				await _userManager.DeleteAsync(user);

			return createUser.Succeeded && addClaims.Succeeded;
		}

		public async Task<ApplicationUser> GetUserByIdAsync(string id) =>
			await _userManager.FindByIdAsync(id);

		public async Task<ApplicationUser> GetUserByEmailAsync(string email) =>
			await _userManager.FindByEmailAsync(email);

		public Task<bool> DeleteUserByIdAsync(string id) => throw new System.NotImplementedException();
		public Task<bool> UpdateUserAsync(string id) => throw new System.NotImplementedException();
	}
}