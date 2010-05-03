using System;
using System.Text.RegularExpressions;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// Used for preprocessing the SQL text prior to sending it to the data source
	/// </summary>
	internal static class SqlCommandText
	{
		private static readonly Regex StartQueryRegex = new Regex(@"^\s*--\s*start\s*query\s*",
		                                                          RegexOptions.IgnoreCase | RegexOptions.Multiline);

		public static string Sanitize(string rawSql)
		{
			if (String.IsNullOrEmpty(rawSql))
			{
				return string.Empty;
			}

			var match = StartQueryRegex.Match(rawSql);
			if (match.Success)
			{
				return rawSql.Substring(match.Index);
			}

			// match not found, return original SQL
			return rawSql;
		}
	}
}