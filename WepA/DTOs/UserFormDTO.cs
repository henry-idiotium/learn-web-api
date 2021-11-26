using System;
using System.ComponentModel.DataAnnotations;

namespace WepA.DTOs
{
	public class UserFormDTO
	{
		public string Id { get; set; }
		public string UserName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		public string Address { get; set; }
		public DateTime? DateOfBirth { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string ConfirmPassword { get; set; }
	}
}