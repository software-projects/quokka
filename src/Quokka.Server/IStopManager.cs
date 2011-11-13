using System;
using System.Threading;

namespace Quokka.Server
{
	/// <summary>
	/// Interface for coordinating the module stopping.
	/// </summary>
	/// <remarks>
	/// An object implementing this interface is available from the IoC
	/// container.
	/// </remarks>
	public interface IStopManager
	{
		/// <summary>
		/// Request the module to stop.
		/// </summary>
		/// <remarks>
		/// Call this method when a fatal error has occurred that requires
		/// restarting the module's AppDomain.
		/// </remarks>
		void RequestStop();

		/// <summary>
		/// True if the <see cref="RequestStop"/> method has been called.
		/// </summary>
		bool HasStopBeenRequested { get; }

		/// <summary>
		/// A <see cref="WaitHandle"/> that will be set when the <see cref="RequestStop"/>
		/// method is called.
		/// </summary>
		/// <remarks>
		/// This becomes useful when waiting for long-running operations to complete.
		/// </remarks>
		WaitHandle StopWaitHandle { get; }

		/// <summary>
		/// Register an <see cref="Action"/> that will be called after stop has
		/// been requested for the module.
		/// </summary>
		/// <param name="stopAction">Action that will be called when module stop has been requested</param>
		/// <remarks>
		/// All stop actions are executed on a worker thread shortly after <see cref="RequestStop"/>
		/// has been called.
		/// </remarks>
		void RegisterStopAction(Action stopAction);

		/// <summary>
		/// Called by a thread in the AppDomain if it wants to delay the AppDomain being unloaded.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="IDisposable"/>. When this object is disposed, the AppDomain can be unloaded.
		/// </returns>
		/// <remarks>
		/// This delays unloading the AppDomain, but not indefinitely. 
		/// </remarks>
		IDisposable DelayStop();
	}
}
