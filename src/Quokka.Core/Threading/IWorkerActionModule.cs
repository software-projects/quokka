using System;

namespace Quokka.Threading
{
	public interface IWorkerActionModule
	{
		/// <summary>
		/// 	Called before the <see cref = "Worker" /> runs its <see cref = "WorkerAction" /> action.
		/// </summary>
		/// <remarks>
		///		The <see cref="Before"/> method for all <see cref="IWorkerActionModule"/> modules will
		///		be called prior to performing the <see cref="WorkerAction"/> action.
		/// </remarks>
		void Before();

		/// <summary>
		/// 	Called on successfuly completion of the <see cref = "WorkerAction" /> action. This method
		/// 	is not called if an exception is thrown.
		/// </summary>
		void After();

		/// <summary>
		/// 	Called if the <see cref = "WorkerAction" /> throws an exception while performing its work.
		/// </summary>
		/// <param name = "ex">The exception thrown</param>
		void Error(Exception ex);

		/// <summary>
		/// 	Called on completion of the <see cref = "WorkerAction" />, regardless of whether an
		/// 	exception was thrown or not.
		/// </summary>
		void Finished();
	}
}