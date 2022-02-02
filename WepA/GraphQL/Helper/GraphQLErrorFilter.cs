using System;
using System.Collections.Generic;
using System.Net;
using HotChocolate;
using WepA.Helpers;
using WepA.Helpers.ResponseMessages;

namespace WepA.GraphQL.Helper
{
	public class GraphQLErrorFilter : IErrorFilter
	{
		public IError OnError(IError error)
		{
			var code = ParseToErrorCodes(error.Exception);
			var message = error.Exception is HttpStatusException
				? error.Exception.Message
				: ErrorResponseMessages.UnexpectedError;

			return ErrorBuilder.FromError(error)
				.SetMessage(message).SetCode(code)
				.RemoveException().ClearExtensions()
				.RemovePath().ClearLocations()
				.Build();
		}

		private static string ParseToErrorCodes(Exception exception)
		{
			var dict = new Dictionary<HttpStatusCode, string>
			{
				{ HttpStatusCode.InternalServerError, ErrorCodes.Server.RequestInvalid },
				{ HttpStatusCode.Unauthorized, ErrorCodes.Authentication.NotAuthorized },
				{ HttpStatusCode.BadRequest, ErrorCodes.Server.RequestInvalid },
			};

			return dict[
				exception is HttpStatusException ex
					? ex.Status
					: HttpStatusCode.InternalServerError
			];
		}
	}
}