namespace WepA.Helpers.Messages
{
	public static class ErrorResponseMessages
	{
		public const string ServerError = "Server error";
		public const string GenericError = "Something went wrong";
		public const string FailedVerifyEmail = "Failed to verify email";
		public const string FailedLogin = "Username or password is incorrect";
		public const string Unauthorized = "Unauthorized";
		public const string InvalidRequest = "Invalid request";
		public const string NotFoundUser = "User not found";
		public const string UserAlreadyExists = "This username or email already exists";
		public const string EmailNotVerify = "Please verify email";
		public const string InvalidRefreshToken = "Invalid refresh token";
		public const string ExpiredToken = "Token expired";
	}
}