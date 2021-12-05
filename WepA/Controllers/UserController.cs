using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WepA.DTOs;
using WepA.Interfaces.Services;
using WepA.Interfaces.Utils;
using WepA.Models;
using WepA.Services.Interfaces;

namespace WepA.Controllers
{
	[Authorize(Roles = "admin")]
	[ApiController]
	[Route("api/[controller]")]
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

		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var userDetails =
					_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDetailsDTO>>(
						_userService.GetUsers());

				return new JsonResult(new
				{
					succeeded = true,
					data = userDetails
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new
				{
					succeeded = false,
					message = ex
				});
			}
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(UserFormDTO input)
		{
			try
			{
				var message = string.Empty;
				var actionResult = false;
				if (ModelState.IsValid)
				{
					var user = _mapper.Map<UserFormDTO, ApplicationUser>(input);
					var result = await _userService.CreateUserAsync(user, input.Password);

				if (getToken)
				{
					var token = await _jwtUtil.GenerateTokenAsync(input.Email, input.Roles);
					return Ok(new { token });
				}

				return new JsonResult(new
				{
					succeeded = true,
					actionResult,
					message
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new
				{
					succeeded = false,
					message = ex
				});
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetDetails(string id)
		{
			try
			{
				return new JsonResult(new
				{
					succeeded = true,
					data = await _userService.GetUserByIdAsync(id)
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new
				{
					succeeded = false,
					message = ex
				});
			}
		}
	}
}