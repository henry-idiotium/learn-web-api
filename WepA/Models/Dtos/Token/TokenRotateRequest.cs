namespace WepA.Models.Dtos.Token.Token
{
	public class TokenRotateRequest
	{
		public TokenRotateRequest(string accessToken, string refreshToken)
		{
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}

		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}