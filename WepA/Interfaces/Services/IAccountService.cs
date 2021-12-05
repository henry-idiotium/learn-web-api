using System.Threading.Tasks;
using WepA.Models;

namespace WepA.Interfaces.Services
{
	public interface IAccountService
	{
		Task<bool> LoginAccountAsync(Account account);
		Task<bool> RegisterAccountAsync(ApplicationUser user, string password);
	}
}