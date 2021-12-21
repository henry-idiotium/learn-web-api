using WepA.Models.Dtos.User;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WepA.Helpers.Attributes;

namespace WepA.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IMapper _mapper;

		public UserController(IUserService userService, IMapper mapper)
		{
			_userService = userService;
			_mapper = mapper;
		}

		[JwtAuthorize]
		[HttpGet]
		public IActionResult GetAll()
		{
			var users = _mapper
				.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDetailsResponse>>(_userService.GetAll());
			return Ok(users);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] ManageUserRequest model)
		{

			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.UserName ??= model.Email;
			var user = _mapper.Map<ManageUserRequest, ApplicationUser>(model);
			await _userService.CreateAsync(user, model.Password, model.Roles);

			return Ok(new { message = "User created" });
		}

		[HttpGet]
		public async Task<IActionResult> GetDetails(string id)
		{
			var user = await _userService.GetByIdAsync(id);
			return Ok(user);
		}
	}
}