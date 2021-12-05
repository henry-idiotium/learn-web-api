using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WepA.Models;

namespace WepA.Extensions.Helpers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			// Skip authorization if action is decorated with [AllowAnonymous] attribute
			var allowAnonymous = context.ActionDescriptor.EndpointMetadata
				.OfType<AllowAnonymousAttribute>().Any();

			if (allowAnonymous) return;

			// authorization
			var user = (ApplicationUser)context.HttpContext.Items["ApplicationUser"];
			if (user is null)
			{
				context.Result = new JsonResult(new { message = "Unauthorized" })
				{
					StatusCode = StatusCodes.Status401Unauthorized
				};
			}
		}
	}
}