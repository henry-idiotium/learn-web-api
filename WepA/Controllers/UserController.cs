using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WepA.DTOs;
using WepA.Interfaces.Services;
using WepA.Interfaces.Utils;
using WepA.Models;

namespace WepA.Controllers
{
	[Authorize(Roles = "admin")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class UserController : ControllerBase
	{
		private readonly ILogger<UserController> _logger;
		private readonly IUserService _userService;
		private readonly IMapper _mapper;
		private readonly IJwtUtil _jwtUtil;

		public UserController(
			IUserService userService,
			IMapper mapper,
			ILogger<UserController> logger,
			IJwtUtil jwtUtil)
		{
			_userService = userService;
			_mapper = mapper;
			_logger = logger;
			_jwtUtil = jwtUtil;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			try
			{
				var users = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDetailsDTO>>(
					_userService.GetUsers());
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest(new { message = "Something went wrong!" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserFormDTO input, bool getToken)
		{
			try
			{
				if (!ModelState.IsValid) return BadRequest(ModelState);

				input.UserName ??= input.Email;
				var user = _mapper.Map<UserFormDTO, ApplicationUser>(input);
				var result = await _userService.CreateUserAsync(user, input.Password, input.Roles);
				if (!result) return BadRequest(new { message = "Invalid!" });

				if (getToken)
				{
					var token = await _jwtUtil.GenerateTokenAsync(input.Email, input.Roles);
					return Ok(new { token });
				}
				return Ok(new { message = "Successful!" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest(new { message = "Something went wrong!" });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetDetails(string id)
		{
			try
			{
				var user = await _userService.GetUserByIdAsync(id);
				return Ok(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest(new { message = "Something went wrong!" });

			}
		}
	}
}