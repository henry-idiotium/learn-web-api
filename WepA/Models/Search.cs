using System.Collections.Generic;

namespace WepA.Models
{
	public class Search
	{
		public string Filter { get; set; }
		public List<string> FilterBy { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public List<string> OrderBy { get; set; }
	}
}