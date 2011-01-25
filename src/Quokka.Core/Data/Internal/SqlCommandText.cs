#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.Text.RegularExpressions;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Used for preprocessing the SQL text prior to sending it to the data source
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