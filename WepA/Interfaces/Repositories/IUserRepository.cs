using System.Threading.Tasks;
using WepA.Models.Domains;

namespace WepA.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<bool> AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken);
		ApplicationUser GetByRefreshToken(string token);
		RefreshToken GetRefreshToken(string token);
		Task<bool> RemoveOutdatedRefreshTokensAsync(ApplicationUser user);
		Task<bool> RevokeRefreshTokenAsync(RefreshToken token, string reason = null, string replacedByToken = null);
		Task<bool> RevokeRefreshTokenDescendantsAsync(RefreshToken token, ApplicationUser user, string reason);
		bool ValidateUserExistence(ApplicationUser user);
	}
}