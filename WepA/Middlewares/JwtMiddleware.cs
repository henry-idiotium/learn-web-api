using WepA.Interfaces.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace WepA.Middlewares
{
	public static class JwtMiddlewareExt
	{
		public static void UseJwtMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<JwtMiddleware>();
		}
	}

	public class JwtMiddleware
	{
		private readonly RequestDelegate _next;

		public JwtMiddleware(RequestDelegate next) => _next = next;

		public async Task InvokeAsync(HttpContext context, IUserService userService, IJwtService jwtService)
		{
			// Reading the AuthHeader which is signed with JWT
			var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			try
			{
				var userId = jwtService.Validate(token);
				if (!string.IsNullOrWhiteSpace(userId))
				{
					// Attach user to context on successful jwt validation
					context.Items["ApplicationUser"] = await userService.GetByIdAsync(userId);
				}
			}
			catch { } // If jwt validation fails then do nothing
			await _next(context);
		}
	}
}