using WepA.Models.Domains;

namespace WepA.Models.Dtos
{
	public class AuthenticateResponse
	{
		public AuthenticateResponse(AccessToken accessToken, string refreshToken)
		{
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}

		public AccessToken AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}