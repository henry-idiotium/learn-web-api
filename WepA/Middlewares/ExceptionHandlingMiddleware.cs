using WepA.Models.Dtos;
using WepA.Helpers.Messages;
using WepA.Helpers;
using System.Threading.Tasks;
using System.Net;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

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
			{
				code = (int)httpException.Status;
				context.Response.Headers.Add("X-Log-Status-Code", httpException.Status.ToString());
				context.Response.Headers.Add("X-Log-Message", exception.Message);
			}

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = code;

			var response = JsonConvert.SerializeObject(new Response
			{
				Message = (exception.Message != null) && (exception is HttpStatusException) ?
					exception.Message : ErrorResponseMessages.GenericError
			});
			await context.Response.WriteAsync(response);
		}
	}
}
