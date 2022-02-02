namespace WepA.Helpers.ResponseMessages
{
	public static class ErrorResponseMessages
	{
		public const string EmailNotVerify = "Please verify email";
		public const string ExpiredToken = "Token expired";
		public const string FailedLogin = "Email or password is incorrect";
		public const string FailedVerifyEmail = "Failed to verify email";
		public const string UnexpectedError = "Something went wrong";
		public const string FailedRequest = "Failed request";
		public const string InvalidTokens = "Invalid tokens";
		public const string InvalidRequest = "Invalid request";
		public const string NotFoundUser = "User not found";
		public const string PasswordNotMatch = "Submitted passwords didn't match";
		public const string ServerError = "Server error";
		public const string Unauthorized = "Unauthorized";
		public const string UserAlreadyExists = "This username or email already exists";
	}
}