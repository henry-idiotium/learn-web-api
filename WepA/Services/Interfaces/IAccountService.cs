using System.Threading.Tasks;
using WepA.Common;
using WepA.Models;

namespace WepA.Services.Interfaces
{
	public interface IAccountService
	{
		Task<ResultStateToken> LoginAccountAsync(Account input);
		Task<ResultStateToken> RegisterAccountAsync(ApplicationUser input, string password);
	}
}