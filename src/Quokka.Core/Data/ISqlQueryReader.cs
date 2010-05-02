using System;

namespace Quokka.Data
{
	/// <summary>
	/// Provides a mechanism for reading a large number of rows from the
	/// query without creating a large number of record objects.
	/// </summary>
	/// <typeparam name="T">
	/// Type of record object returned from the <see cref="SqlQuery"/>
	/// query.
	/// </typeparam>
	public interface ISqlQueryReader<T> : IDisposable where T : class
	{
		T Record { get; }
		bool Read();
	}
}