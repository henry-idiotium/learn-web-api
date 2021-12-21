using System.Collections.Generic;
using System.Security.Claims;
using WepA.Models.Domains;
using WepA.Models.Dtos;

namespace WepA.Interfaces.Services
{
	public interface IJwtService
	{
		AccessToken GenerateAccessToken(IEnumerable<Claim> claims);
		RefreshToken GenerateRefreshToken(string userId);
		ClaimsPrincipal GetClaimsPrincipal(string token);
		string Validate(string token);
	}
}