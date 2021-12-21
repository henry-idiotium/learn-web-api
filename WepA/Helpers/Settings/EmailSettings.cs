namespace WepA.Helpers.Settings
{
	public class EmailSettings
	{
		public string FromMail { get; set; }
		public string DisplayName { get; set; }
		public string Password { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public bool IsEnableSsl { get; set; }
	}
}