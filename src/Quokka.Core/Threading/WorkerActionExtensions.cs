using System.Collections.Generic;
using Quokka.Data;

namespace Quokka.Threading
{
	/// <summary>
	/// Some common examples of using a <see cref="WorkerAction"/>.
	/// </summary>
	public static class WorkerActionExtensions
	{
		/// <summary>
		/// Execute an <see cref="SqlQuery{T}"/> query and return a <see cref="List{T}"/> list.
		/// </summary>
		/// <typeparam name="T">
		///		Type of objects that will be returned in the list.
		/// </typeparam>
		/// <param name="builder">
		///		The builder for the <see cref="WorkerAction"/>.
		/// </param>
		public static WorkerAction.ICompletionBuilder<List<T>> ExecuteList<T>(this WorkerAction.IDoWorkBuilder builder,
		                                                                      SqlQuery<T> query)
			where T : class, new()
		{
			return builder.DoWork(() => query.ExecuteList());
		}

		public static WorkerAction.ICompletionBuilder<T> ExecuteSingle<T>(this WorkerAction.IDoWorkBuilder builder,
																			  SqlQuery<T> query)
			where T : class, new()
		{
			return builder.DoWork(() => query.ExecuteSingle());
		}
	}
}