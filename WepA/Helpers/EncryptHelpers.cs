using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace WepA.Helpers
{
	public static class EncryptHelpers
	{
		public static string EncodeBase64Url(string sequence) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(sequence));

		public static string DecodeBase64Url(string sequence) => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(sequence));

		public static byte[] EncodeASCII(string sequence) => Encoding.ASCII.GetBytes(sequence);
	}
}