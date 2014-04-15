#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
