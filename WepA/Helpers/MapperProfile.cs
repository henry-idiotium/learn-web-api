using AutoMapper;
using WepA.Models.Domains;
using WepA.Models.Dtos;
using WepA.Models.Dtos.User;

namespace WepA.Helpers
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<RegisterRequest, ApplicationUser>();
			CreateMap<ManageUserRequest, ApplicationUser>();
			CreateMap<ApplicationUser, UserDetailsResponse>();
		}
	}
}