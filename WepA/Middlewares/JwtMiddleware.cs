using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WepA.Interfaces.Services;
using WepA.Interfaces.Utils;

namespace WepA.Middlewares
{
	public class JwtMiddleware
	{
		private readonly RequestDelegate _next;

		public JwtMiddleware(RequestDelegate next) => _next = next;

		public async Task InvokeAsync(HttpContext context, IJwtUtil jwtUtil)
		{
			// Reading the AuthHeader which is signed with JWT
			var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			try
			{
				var userId = jwtUtil.Validate(token);
				if (!string.IsNullOrWhiteSpace(userId))
				{
					// Attach user to context on successful jwt validation
					context.Items["UserId"] = userId;
				}
			}
			catch { } // If jwt validation fails then do nothing

			await _next(context);
		}
	}
}