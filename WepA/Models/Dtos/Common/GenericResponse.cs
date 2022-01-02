using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WepA.Models.Dtos.Common
{
	public class GenericResponse
	{
		public GenericResponse() { }

		public GenericResponse(string message, bool isSuccess = true)
		{
			IsSuccess = isSuccess;
			Message = message;
			TimeStamp = ConvertDateTime(DateTime.UtcNow);
		}
		GenericResponse(object responseData, string message, bool isSuccess = true)
		{
			ResponseData = responseData;
			Message = message;
			IsSuccess = isSuccess;
			TimeStamp = ConvertDateTime(DateTime.UtcNow);
		}

		public bool IsSuccess { get; private set; }

		public string Message { get; private set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public object ResponseData { get; private set; }

		public string TimeStamp { get; private set; }

		public GenericResponse For<T>(T responseData, string message, bool isSuccess = true) =>
			new(responseData, message, isSuccess);

		static string ConvertDateTime(DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss tt");
	}
}