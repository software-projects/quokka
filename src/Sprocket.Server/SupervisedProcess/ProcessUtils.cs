using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Sprocket.Server.SupervisedProcess
{
	public static class ProcessUtils
	{
		/// <summary>
		/// Open a process as a wait handle, which allows waiting for that process to terminate
		/// </summary>
		/// <param name="processId">Process ID of the process</param>
		/// <returns>A wait handle for the process</returns>
		public static WaitHandle GetProcessWaitHandle(int processId)
		{
			SafeWaitHandle safeWaitHandle = OpenProcess(Synchronize, 0, processId);
			if (safeWaitHandle.IsInvalid)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			return new ProcessWaitHandle(safeWaitHandle);
		}

		private const int Synchronize = 0x00100000;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeWaitHandle OpenProcess(int desiredAccess, int inherit, int processId);

		private class ProcessWaitHandle : WaitHandle
		{
			public ProcessWaitHandle(SafeWaitHandle safeWaitHandle)
			{
				SafeWaitHandle = safeWaitHandle;
			}
		}
	}
}
