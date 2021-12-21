using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WepA.Helpers;
using WepA.Helpers.Messages;
using WepA.Models.Dtos;

namespace WepA.Middlewares
{
	public static class ExceptionHandlingMiddlewareExt
	{
		public static void UseHttpStatusExceptionHandling(this IApplicationBuilder app)
		{
			app.UseMiddleware<HttpStatusExceptionHandlerMiddleware>();
		}
	}

	public class HttpStatusExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public HttpStatusExceptionHandlerMiddleware(RequestDelegate next,
			ILogger<HttpStatusExceptionHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				if (ex is not HttpStatusException)
					_logger.LogError(ex.Message);

				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = (int)HttpStatusCode.InternalServerError; // Internal Server Error by default
			if (exception is HttpStatusException httpException)
				code = (int)httpException.Status;

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = code;

			if (exception is HttpStatusException)
				await context.Response.WriteAsync((new
				{
					message = exception.Message
				}).ToString());
			else
				await context.Response.WriteAsync((new
				{
					message = ErrorResponseMessages.ServerError
				}).ToString());
		}
	}
}
