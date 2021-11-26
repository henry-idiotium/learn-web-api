using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WepA.DTOs;
using WepA.Models;
using WepA.Services.Interfaces;

namespace WepA.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;

		public AccountController(IMapper mapper, IAccountService accountService)
		{
			_mapper = mapper;
			_accountService = accountService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(AccountRegisterDTO input)
		{
			try
			{
				var message = string.Empty;
				var actionResult = false;
				if (ModelState.IsValid)
				{
					input.UserName ??= input.Email;
					var user = _mapper.Map<AccountRegisterDTO, ApplicationUser>(input);
					var result = await _accountService.RegisterAccountAsync(user, input.Password);

					actionResult = true;
					message = !result.succeeded ?
						string.Join("; ", result.errors) :
						"Register successfully!";
				}
				else
				{
					message = string.Join("; ", ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage));
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

		[HttpPost("Login")]
		public async Task<IActionResult> Login(AccountLoginDTO input)
		{
			try
			{
				var message = string.Empty;
				var actionResult = false;
				if (ModelState.IsValid)
				{
					var user = _mapper.Map<AccountLoginDTO, Account>(input);
					var result = await _accountService.LoginAccountAsync(user);

					actionResult = true;
					message = !result.succeeded ?
						string.Join("; ", result.errors) :
						"Login successfully!";
				}
				else
				{
					message = string.Join("; ", ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage));
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
	}
}