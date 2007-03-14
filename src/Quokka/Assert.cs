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
	}
}