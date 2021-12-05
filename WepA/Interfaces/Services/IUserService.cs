using System.Collections.Generic;
using System.Threading.Tasks;
using WepA.Models;

namespace WepA.Interfaces.Services
{
	public interface IUserService
	{
		IEnumerable<ApplicationUser> GetUsers();
		Task<bool> CreateUserAsync(ApplicationUser user, string password, List<string> roles);
		Task<ApplicationUser> GetUserByIdAsync(string id);
		Task<ApplicationUser> GetUserByEmailAsync(string email);
		Task<bool> DeleteUserByIdAsync(string id);
		Task<bool> UpdateUserAsync(string id);
	}
}