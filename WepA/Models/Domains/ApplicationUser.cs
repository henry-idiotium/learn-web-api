using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WepA.Models.Domains
{
	public class ApplicationUser : IdentityUser
	{
		[StringLength(50)]
		[Required]
		public string FirstName { get; set; }

		[StringLength(50)]
		[Required]
		public string LastName { get; set; }

		[DataType(DataType.Date)]
		public DateTime? DateOfBirth { get; set; }

		[StringLength(250)]
		public string Address { get; set; }

		[JsonIgnore]
		public List<RefreshToken> RefreshTokens { get; set; }
	}
}