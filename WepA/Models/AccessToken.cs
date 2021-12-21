namespace WepA.Models.Dtos
{
	public class AccessToken
	{
		public AccessToken(string token, int expireIn)
		{
			Token = token;
			ExpireIn = expireIn;
		}

		public string Token { get; set; }
		public int ExpireIn { get; set; }
	}
}