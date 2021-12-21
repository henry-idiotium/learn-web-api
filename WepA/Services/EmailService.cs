using System.Net;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using WepA.Helpers;
using WepA.Helpers.Messages;
using WepA.Interfaces.Services;
using WepA.Models.Domains;
using WepA.Models.Dtos;

namespace WepA.Services
{
	public class EmailService : IEmailService
	{
		private const string _authPath = "https://localhost:5001/api/auth";
		private readonly IFluentEmail _fluentEmail;
		private readonly ILogger<EmailService> _logger;

		public EmailService(IFluentEmail fluentEmail, ILogger<EmailService> logger)
		{
			_fluentEmail = fluentEmail;
			_logger = logger;
		}

		public async Task SendConfirmEmailAsync(ApplicationUser user, string code)
		{
			var url = $"{_authPath}/confirmemail/{user.Id}/{code}";
			var email = _fluentEmail
				.To(user.Email)
				.Subject("Welcome to WepA")
				.UsingTemplateFromFile(
					filename: "Templates/ConfirmationEmailTemplate.cshtml",
					model: new { User = user, Url = url });

			var result = await email.SendAsync();
			if (!result.Successful)
			{
				_logger.LogError($"Failed to send an email \"{user.Email}\".", result.ErrorMessages);
				throw new HttpStatusException(
					status: HttpStatusCode.InternalServerError,
					message: ErrorResponseMessages.ServerError);
			}
		}
	}
}