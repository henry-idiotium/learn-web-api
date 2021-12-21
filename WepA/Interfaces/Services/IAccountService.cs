using WepA.Models.Dtos;
using WepA.Models.Domains;
using System.Threading.Tasks;

namespace WepA.Interfaces.Services
{
	public interface IAccountService
	{
		Task<AuthenticateResponse> LoginAsync(LoginRequest account);
		Task RegisterAsync(ApplicationUser user, string password);
		Task ConfirmEmailAsync(string userId, string code);
	}
}