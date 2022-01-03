using System.Collections.Generic;
using System.Threading.Tasks;
using Sieve.Models;
using WepA.Models.Dtos.Token;
using WepA.Models.Dtos.Common;
using WepA.Models.Entities;

namespace WepA.Interfaces.Services
{
	public interface IUserService
	{
		Task AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken);
		Task CreateAsync(CreateUserRequest model);
		Task MockCreateAsync(List<CreateUserRequest> models);
		Task DeleteAsync(string userId);
		Task<ApplicationUser> GetByEmailAsync(string email);
		Task<ApplicationUser> GetByIdAsync(string userId);
		ObjectListResponse GetList(SieveModel search);
		Task RevokeRefreshToken(string token);
		Task<AuthenticateResponse> RotateTokensAsync(TokenRotateRequest model);
		Task UpdateAsync(ApplicationUser user);
		bool ValidateExistence(ApplicationUser user);
	}
}