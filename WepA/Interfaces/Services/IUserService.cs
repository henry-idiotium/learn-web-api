using System.Collections.Generic;
using System.Threading.Tasks;
using WepA.Models.Domains;
using WepA.Models.Dtos;
using WepA.Models.Dtos.Token;

namespace WepA.Interfaces.Services
{
	public interface IUserService
	{
		IEnumerable<ApplicationUser> GetAll();
		Task<ApplicationUser> GetByIdAsync(string userId);
		Task<ApplicationUser> GetByEmailAsync(string email);
		Task CreateAsync(ApplicationUser user, string password, List<string> roles);
		Task DeleteAsync(string userId);
		Task UpdateAsync(ApplicationUser user);
		Task<AuthenticateResponse> RotateTokensAsync(TokenRotateRequest model);
		Task RevokeRefreshToken(string token);
		Task AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken);
		bool ValidateUserExistence(ApplicationUser user);
	}
}