using System;
using System.Collections.Generic;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Simple class for storing objects by a string alias, which may be <c>null</c>.
	/// </summary>
	public class AliasDictionary<T> where T : class
	{
		private readonly Dictionary<string, T> _dict = new Dictionary<string, T>();
		private T _nullAliasValue;

		public T Find(string alias)
		{
			T result;
			if (alias == null)
			{
				result = _nullAliasValue;
			}
			else
			{
				_dict.TryGetValue(alias, out result);
			}
			return result;
		}

		public void Save(string alias, T value)
		{
			if (alias == null)
			{
				_nullAliasValue = value;
			}
			else
			{
				_dict[alias] = value;
			}
		}
	}
}
