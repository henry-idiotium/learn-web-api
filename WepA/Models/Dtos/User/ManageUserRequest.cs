using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WepA.Models.Dtos.User
{
	public class ManageUserRequest
	{
		public string UserName { get; set; }

		[Required]
		public string Email { get; set; }

		public string Address { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public List<string> Roles { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string ConfirmPassword { get; set; }
	}
}