using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Quokka.Util
{
	/// <summary>
	/// String utils for .NET 4.0, .NET 3.5 compatibility
	/// </summary>
	/// <remarks>
	/// Tempting to make public, but will make internal at the moment,
	/// because StringUtils is probably something many projects already
	/// have.
	/// </remarks>
	internal static class StringUtils
	{
		#region IsNullOrWhiteSpace
#if NET40
		public static bool IsNullOrWhiteSpace(string s)
		{
			return string.IsNullOrWhiteSpace(s);
		}
#endif

#if NET35
		private static readonly Regex WhiteSpaceRegex = new Regex(@"^\s*$");
		public static bool IsNullOrWhiteSpace(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return true;
			}
			return WhiteSpaceRegex.IsMatch(s);
		}
#endif
		#endregion
	}
}
