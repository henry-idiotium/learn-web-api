using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WepA.Interfaces.Services;
using WepA.Models;

namespace WepA.Services
{
	public class AccountService : IAccountService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public AccountService(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<bool> LoginAccountAsync(Account account)
		{
			var loginResult = await _signInManager
				.PasswordSignInAsync(account.Email, account.Password, false, false);
			if (!loginResult.Succeeded) return false;

			var user = await _userManager.FindByEmailAsync(account.Email);
			var claims = await _userManager.GetClaimsAsync(user);

			return claims != null;
		}

		public async Task<bool> RegisterAccountAsync(ApplicationUser user, string password)
		{
			var createUser = await _userManager.CreateAsync(user, password);
			if (!createUser.Succeeded) return false;

			var newUser = await _userManager.FindByEmailAsync(user.Email);
			await _userManager.AddToRoleAsync(newUser, "customer");

			var addClaims = await _userManager.AddClaimsAsync(newUser,
				new List<Claim>
				{
					new Claim(ClaimTypes.Role, "customer"),
					new Claim(ClaimTypes.NameIdentifier, newUser.Id),
					new Claim(ClaimTypes.Email, newUser.Email),
					new Claim(ClaimTypes.Name, newUser.UserName)
				});

			if (!(createUser.Succeeded && addClaims.Succeeded))
				await _userManager.DeleteAsync(user);

			return createUser.Succeeded && addClaims.Succeeded;
		}
	}
}