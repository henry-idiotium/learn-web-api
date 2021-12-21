using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WepA.Helpers.Messages;
using WepA.Models.Domains;
using WepA.Models.Dtos;

namespace WepA.Helpers.Attributes
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
			if (user == null)
			{
				context.Result = new JsonResult(new { message = ErrorResponseMessages.GenericError })
				{
					StatusCode = StatusCodes.Status400BadRequest
				};
			}
		}
	}
}