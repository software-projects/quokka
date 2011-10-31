using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using log4net;
using Quokka.Diagnostics;
using System.Collections.Generic;

namespace Sprocket.Service
{
	/// <summary>
	/// This class looks after the starting and runtime supervision of child processes
	/// </summary>
	public class ProcessSupervisor
	{
		/// <summary>
		/// Minimum time between restarting a child process (msec)
		/// </summary>
		private const int MinRetryTime = 100;

		/// <summary>
		/// Maximum time between restarting a child process (msec)
		/// </summary>
		private const int MaxRetryTime = 64000;

		/// <summary>
		/// Maximum time that a child process has to stop itself (msec)
		/// </summary>
		public const int MaxProcessStopTime = 15000;

		private static readonly ILog Log = LogManager.GetLogger(typeof (ProcessSupervisor));
		private readonly EventWaitHandle _eventWaitHandle;
		private readonly string _eventWaitHandleName;
		private volatile bool _stopRequested;
		private Thread _thread;
		private static int _staticInstanceNumber;

		public bool DebugMode { get; set; }

		public string ProgramName { get; set; }
		public string CommandLineArgs { get; set; }

		public ProcessSupervisor()
		{
			var processId = Process.GetCurrentProcess().Id;
			var instanceNumber = Interlocked.Increment(ref _staticInstanceNumber);
			_eventWaitHandleName = "Sprocket.Service.StopEvent." + processId + "." + instanceNumber;
			_eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, _eventWaitHandleName);
			CommandLineArgs = string.Empty;
		}

		public void StartProcess()
		{
			_eventWaitHandle.Reset();
			_stopRequested = false;

			try
			{
				Thread thread = new Thread(StartChildProcessThreadMain);
				thread.Start();
				_thread = thread;
			}
			catch (Exception ex)
			{
				Log.Error("Cannot start sub-process: " + ProgramName + " " + CommandLineArgs, ex);
				throw;
			}
		}

		public void StopProcess()
		{
			if (_thread == null)
			{
				return;
			}

			_stopRequested = true;
			_eventWaitHandle.Set();
			_thread.Join();
		}

		private void Sleep(int milliseconds)
		{
			_eventWaitHandle.WaitOne(milliseconds);
		}

		private void StartChildProcessThreadMain(object parameter)
		{
			int retryTime = MinRetryTime;

			string args = CommandLineArgs ?? string.Empty;
			Process process = null;

			while (!_stopRequested)
			{
				while (process == null)
				{
					Sleep(retryTime);
					if (_stopRequested)
						break;

					try
					{
						var startInfo = new ProcessStartInfo(GetExeFilePath(), args)
						                	{
						                		UseShellExecute = false,
						                	};

						startInfo.EnvironmentVariables["STOP_EVENT_NAME"] = _eventWaitHandleName;
						startInfo.EnvironmentVariables["PARENT_PROCESS_ID"] = Process.GetCurrentProcess().Id.ToString();

						process = Process.Start(startInfo);
						Log.InfoFormat("Started child process {0}: {1}", process.Id, args);
					}
					catch (Exception ex)
					{
						if (ex.IsCorruptedStateException())
						{
							throw;
						}
						Log.Error("Cannot start process: " + ProgramName + " " + args, ex);
						retryTime = IncreaseRetryTime(retryTime);
					}
				}


				while (process != null && !_stopRequested)
				{
					var stopwatch = Stopwatch.StartNew();

					// Wait for the process to stop or a stop request, whichever comes first
					using (var processWaitHandle = process.GetWaitHandle())
					{
						var waitHandles = new[] {_eventWaitHandle, processWaitHandle};
						WaitHandle.WaitAny(waitHandles);
					}

					stopwatch.Stop();

					if (_stopRequested)
					{
						break;
					}

					retryTime = GetRetryTimeFromRunningTime(retryTime, stopwatch.ElapsedMilliseconds);

					// At this point the process should have stopped.
					if (process.WaitForExit(1000))
					{
						// the process exited
						if (process.ExitCode == 0)
						{
							Log.InfoFormat("Process {0} stopped with exit code {1}",
							               Thread.CurrentThread.Name, process.ExitCode);

							// if a process terminates normally, restart in the minimum time
							retryTime = MinRetryTime;
						}
						else
						{
							Log.WarnFormat("Process {0} stopped with exit code {1}",
							               Thread.CurrentThread.Name, process.ExitCode);
							retryTime = IncreaseRetryTime(retryTime);
						}

						process.Dispose();
						process = null;
					}
					else
					{
						Log.Warn("Expected process to be stopped, but it was still running");
					}
				}
			}

			if (process != null)
			{
				if (!process.WaitForExit(MaxProcessStopTime))
				{
					Log.ErrorFormat("Process {0} has failed to stop correctly", Thread.CurrentThread.Name);

					// We don't do anything drastic like kill the process here, as we know that
					// there is a thread in the child process that will terminate when it detects that
					// this process has terminated.
					//process.Kill();
				}

				process.Dispose();
			}
		}

		private static int IncreaseRetryTime(int retryTime)
		{
			if (retryTime < MinRetryTime)
			{
				retryTime = MinRetryTime;
			}

			retryTime *= 2;

			if (retryTime > MaxRetryTime)
			{
				retryTime = MaxRetryTime;
			}

			return retryTime;
		}

		private static int GetRetryTimeFromRunningTime(int retryTime, long runningTime)
		{
			if (runningTime > MaxRetryTime)
			{
				return MinRetryTime;
			}

			return IncreaseRetryTime(retryTime);
		}


		private string GetExeFilePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProgramName);
		}
	}
}
