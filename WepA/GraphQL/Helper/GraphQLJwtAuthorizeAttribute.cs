using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace WepA.GraphQL.Helper
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class GraphQLJwtAuthorizeAttribute : ObjectFieldDescriptorAttribute
	{
		public override void OnConfigure(IDescriptorContext context,
			IObjectFieldDescriptor descriptor,
			MemberInfo member)
		{
			descriptor.Use(next => async context =>
			{
				await next(context);
			});
		}
	}
}