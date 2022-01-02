namespace WepA.Models.Dtos.Token
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