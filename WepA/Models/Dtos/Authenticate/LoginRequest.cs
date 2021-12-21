using System.ComponentModel.DataAnnotations;

namespace WepA.Models.Dtos
{
	public class LoginRequest
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}