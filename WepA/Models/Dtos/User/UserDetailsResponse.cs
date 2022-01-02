using System;
using System.Text.Json.Serialization;
using Mapster;
using Sieve.Attributes;

namespace WepA.Models.Dtos.Token.User
{
	public class UserDetailsResponse
	{
		[JsonPropertyName("id")]
		public string EncodedId { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string UserName { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string Email { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string FirstName { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string LastName { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string Address { get; set; }

		public DateTime? DateOfBirth { get; set; }
	}
}