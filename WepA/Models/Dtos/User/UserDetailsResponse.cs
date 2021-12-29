using System;
using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;

namespace WepA.Models.Dtos.User
{
	public class UserDetailsResponse
	{
		[JsonIgnore]
		public string NakedId { get; set; }

		[Ignore]
		[JsonPropertyName("id")]
		public string EncodedId { get; set; }

		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public DateTime? DateOfBirth { get; set; }
	}
}