using WepA.Models.Dtos;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using WepA.Helpers;
using WepA.Helpers.Messages;

namespace WepA.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AuthController : ControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;

		public AuthController(IAccountService accountService, IMapper mapper)
		{
			_accountService = accountService;
			_mapper = mapper;
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.SelectMany(k => k.Value.Errors));

			var authenticateResponse = await _accountService.LoginAsync(model);

			return Ok(authenticateResponse);
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.SelectMany(x => x.Value.Errors));

			model.UserName ??= model.Email;
			var user = _mapper.Map<RegisterRequest, ApplicationUser>(model);
			await _accountService.RegisterAsync(user, model.Password);

			return Ok(new { message = "User Registered" });
		}

		[HttpPost("{userId}/{code}")]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			await _accountService.ConfirmEmailAsync(userId, code);
			return Ok(new { message = "Email confirmed" });
		}
	}
}