using System.Collections.Generic;
using System.Threading.Tasks;
using WepA.Common;
using WepA.Models;

namespace WepA.Services.Interfaces
{
	public interface IUserService
	{
		IEnumerable<ApplicationUser> GetUsers();
		Task<ResultStateToken> CreateUserAsync(ApplicationUser user, string password);
		Task<ApplicationUser> GetUserByIdAsync(string id);
		Task<ResultStateToken> DeleteUserByIdAsync(string id);
		Task<ResultStateToken> UpdateUserAsync(string id);
	}
}