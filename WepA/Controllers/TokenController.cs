using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WepA.Helpers;
using WepA.Helpers.Messages;
using WepA.Interfaces.Services;
using WepA.Models.Dtos.Token;

namespace WepA.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly IUserService _userService;

		public TokenController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> Rotate([FromBody] TokenRotateRequest model)
		{
			if (model.AccessToken == null || model.RefreshToken == null)
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRequest);

			var response = await _userService.RotateTokensAsync(model);
			return Ok(response);
		}

		[HttpGet]
		public IActionResult Revoke(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				throw new HttpStatusException(HttpStatusCode.BadRequest,
					ErrorResponseMessages.InvalidRefreshToken);

			_userService.RevokeRefreshToken(refreshToken);
			return Ok(new { message = "Token revoked" });
		}
	}
}