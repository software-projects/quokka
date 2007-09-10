namespace Quokka.Diagnostics
{
	using System;

	/// <summary>
	/// Used for internal consistency checking.
	/// </summary>
	public static class Verify
	{
		public static void ArgumentNotNull(object param, string paramName)
		{
			if (param == null) {
				throw new ArgumentNullException(paramName);
			}
		}

		public static void IsNotNull(object obj)
		{
			if (obj == null) {
				throw new NullReferenceException();
			}
		}
	}
}