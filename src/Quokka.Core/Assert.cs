namespace Quokka
{
	using System;

	internal static class Assert
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