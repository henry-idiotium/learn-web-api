using System;

namespace WepA.DTOs
{
	public class UserDetailsDTO
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