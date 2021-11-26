using System;
using System.ComponentModel.DataAnnotations;

namespace WepA.DTOs
{
	public class AccountLoginDTO
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}