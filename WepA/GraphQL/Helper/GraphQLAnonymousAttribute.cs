using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace WepA.GraphQL.Helper
{
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Method,
		Inherited = true,
		AllowMultiple = true)]
	public class GraphQLAnonymousAttribute : ObjectFieldDescriptorAttribute
	{
		public override void OnConfigure(IDescriptorContext context,
			IObjectFieldDescriptor descriptor,
			MemberInfo member)
		{
			descriptor.Use(next => async context =>
			{/* 
				// Skip authorization if action is decorated with [AllowAnonymous] attribute

				if (allowAnonymous) return;

				// authorization
				var user = (ApplicationUser)context.HttpContext.Items["ApplicationUser"];
				if (user == null)
				{
					context.Result = new JsonResult(new { message = ErrorResponseMessages.UnexpectedError })
					{
						StatusCode = StatusCodes.Status400BadRequest
					};
				} */

				await next(context);
			});
		}

	}
}