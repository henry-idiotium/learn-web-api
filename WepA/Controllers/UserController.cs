using WepA.Models.Dtos.User;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using WepA.Helpers.Attributes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WepA.Models;
using Microsoft.AspNetCore.Authorization;
using WepA.Helpers;

namespace WepA.Controllers
{
	[JwtAuthorize]
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
		public async Task<IActionResult> GetDetails(string encodedUserId)
		{
			var userId = EncryptHelpers.DecodeBase64Url(encodedUserId);
			var user = await _userService.GetByIdAsync(userId);
			return Ok(user);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Search([FromQuery] Search model)
		{
			var users = _userService.GetSpecificUsers(model);
			return Ok(users);
		}
	}
}