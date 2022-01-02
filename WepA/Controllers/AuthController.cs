using WepA.Models.Dtos;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using WepA.Helpers;

namespace WepA.Controllers
{
	[AllowAnonymous]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AuthController : ControllerBase
	{
		private readonly IAccountService _accountService;

		public AuthController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.SelectMany(k => k.Value.Errors));

			var authenticateResponse = await _accountService.LoginAsync(model);

			return Ok(authenticateResponse);
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.SelectMany(x => x.Value.Errors));

			await _accountService.RegisterAsync(model);

			return Ok(new { message = "User Registered" });
		}

		[HttpGet("{userId}/{code}")]
		public async Task<IActionResult> VerifyEmail(string encodedUserId, string encodedConfirmString)
		{
			var userId = EncryptHelpers.DecodeBase64Url(encodedUserId);
			var code = EncryptHelpers.DecodeBase64Url(encodedConfirmString);
			await _accountService.VerifyEmailAsync(userId, code);
			return Ok(new { message = "Email Verified" });
		}
	}
}