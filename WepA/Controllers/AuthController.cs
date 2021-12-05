using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WepA.DTOs;
using WepA.Interfaces.Services;
using WepA.Interfaces.Utils;
using WepA.Models;

namespace WepA.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> _logger;
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;
		private readonly IJwtUtil _jwtUtil;

		public AuthController(
			ILogger<AuthController> logger,
			IAccountService accountService,
			IMapper mapper,
			IJwtUtil jwtUtil)
		{
			_logger = logger;
			_accountService = accountService;
			_mapper = mapper;
			_jwtUtil = jwtUtil;
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Register([FromBody] AccountRegisterDTO input)
		{
			try
			{
				if (!ModelState.IsValid) return BadRequest(ModelState);

				input.UserName ??= input.Email;
				var user = _mapper.Map<AccountRegisterDTO, ApplicationUser>(input);
				var result = await _accountService.RegisterAccountAsync(user, input.Password);
				if (!result) return BadRequest(new { message = "Invalid!" });

				var token = await _jwtUtil.GenerateTokenAsync(input.Email,
					new List<string> { "customer" });
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return new JsonResult(new { message = "Something went wrong!" })
				{
					StatusCode = StatusCodes.Status500InternalServerError
				};
			}
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] AccountLoginDTO input)
		{
			try
			{
				if (!ModelState.IsValid) return BadRequest(ModelState);

				var account = _mapper.Map<AccountLoginDTO, Account>(input);
				var result = await _accountService.LoginAccountAsync(account);
				if (!result) return Unauthorized(new { message = "Invalid!" });

				var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?
					.Split(" ").Last();
				var validateToken = _jwtUtil.Validate(token);
				if (validateToken is not null) return Ok(new { message = "Successful!" });

				token = await _jwtUtil.GenerateTokenAsync(input.Email,
					new List<string> { "customer" });
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return new JsonResult(new { message = "Something went wrong!" })
				{
					StatusCode = StatusCodes.Status500InternalServerError
				};
			}
		}
	}
}