using AutoMapper;
using WepA.DTOs;
using WepA.Models;

namespace WepA.AutoMapperMapping
{
	public class AccountProfile : Profile
	{
		public AccountProfile()
		{
			CreateMap<AccountRegisterDTO, ApplicationUser>();
			CreateMap<AccountLoginDTO, Account>();
		}
	}
}