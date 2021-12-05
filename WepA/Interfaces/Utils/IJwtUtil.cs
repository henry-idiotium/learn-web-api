using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WepA.Interfaces.Utils
{
	public interface IJwtUtil
	{
		Task<string> GenerateTokenAsync(string email, IList<string> roles);
		string Validate(string token);
		ClaimsPrincipal GetPrincipal(string token);
	}
}