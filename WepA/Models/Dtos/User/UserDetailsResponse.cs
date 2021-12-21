using System;

namespace WepA.Models.Dtos.User
{
	public class UserDetailsResponse
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public DateTime? DateOfBirth { get; set; }
	}
}