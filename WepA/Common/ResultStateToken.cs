using System;
using System.Collections.Generic;

namespace WepA.Common
{
	public struct ResultStateToken
	{
		public bool succeeded;
		public string[] errors;

		public ResultStateToken(bool succeeded, string[] errors)
		{
			this.succeeded = succeeded;
			this.errors = errors;
		}

		public override bool Equals(object obj) =>
			obj is ResultStateToken other &&
			succeeded == other.succeeded &&
			EqualityComparer<string[]>.Default.Equals(errors, other.errors);

		public override int GetHashCode() => HashCode.Combine(succeeded, errors);

		public void Deconstruct(out bool succeeded, out string[] errors)
		{
			succeeded = this.succeeded;
			errors = this.errors;
		}

		public static bool operator ==(ResultStateToken left, ResultStateToken right) =>
			left.Equals(right);

		public static bool operator !=(ResultStateToken left, ResultStateToken right) =>
			!(left == right);
	}
}
