using AutoMapper;
using WepA.DTOs;
using WepA.Models;

namespace WepA.AutoMapperMapping
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<ApplicationUser, UserDetailsDTO>();
			CreateMap<UserFormDTO, ApplicationUser>()
				.ForMember(p => p.Id, options => options.Ignore());
		}
	}
}