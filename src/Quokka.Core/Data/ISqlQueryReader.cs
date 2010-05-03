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
		/// <summary>
		/// Contains the data from the current row being processed by the query.
		/// </summary>
		/// <remarks>
		/// Before the first call to <see cref="Read"/> is called, each property
		/// in this record object will contain default values.
		/// </remarks>
		T Record { get; }

		/// <summary>
		/// Read the next row from the query results and populate the <see cref="Record"/>
		/// property with the data from that row.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if data from the next row was retrieved, or <c>false</c> if
		/// there were no more rows to process from the query results.
		/// </returns>
		bool Read();
	}
}