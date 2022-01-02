using System.Collections.Generic;

namespace WepA.Models.Dtos.User
{
	public class UserListResponse
	{
		public UserListResponse(IEnumerable<UserDetailsResponse> rows, int count, int currentPage, int totalPages)
		{
			Rows = rows;
			Count = count;
			CurrentPage = currentPage;
			TotalPages = totalPages;
		}

		public int Count { get; set; }
		public IEnumerable<UserDetailsResponse> Rows { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}