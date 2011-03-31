using System;
using System.Threading;
using log4net;
using Quokka.Diagnostics;

namespace Sprocket.Server.SupervisedProcess
{
    public class MonitoredProcessHarness
    {
    	private static readonly ILog Log = LogManager.GetLogger(typeof (MonitoredProcessHarness));
        private readonly MonitoredService _serviceImplementation;

    	public const string StopEventNameEnvironmentVariable = "STOP_EVENT_NAME";
    	public const string ParentProcessIdEnvironmentVariable = "PARENT_PROCESS_ID";

		public MonitoredProcessHarness(MonitoredService serviceImplementation)
		{
			_serviceImplementation = Verify.ArgumentNotNull(serviceImplementation, "serviceImplementation");
		}

        public void Run(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }

        	StartParentProcessThread();

			var eventName = Environment.GetEnvironmentVariable(StopEventNameEnvironmentVariable);
			using (var stopEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName))
            {
                _serviceImplementation.Start(args);
                stopEvent.WaitOne();
            }
            _serviceImplementation.Stop();
        }

		private static void StartParentProcessThread()
		{
			int parentProcessId;
			if (int.TryParse(Environment.GetEnvironmentVariable(ParentProcessIdEnvironmentVariable), out parentProcessId))
			{
				var parentProcessHandle = ProcessUtils.GetProcessWaitHandle(parentProcessId);
				if (parentProcessHandle == null)
				{
					Log.Error("Cannot open process with id=" + parentProcessId);
				}
				else
				{
					Log.Debug("Parent process id=" + parentProcessId);

					// This needs to be a background thread. If it is not, then it will prevent
					// this process from terminating at the correct time.
					var thread = new Thread(WaitForParentToTerminate) {IsBackground = true};
					thread.Start(parentProcessHandle);
				}
			}
		}

		private static void WaitForParentToTerminate(object param)
		{
			var waitHandle = (WaitHandle) param;
			waitHandle.WaitOne();
			Thread.Sleep(1000);
			Log.Fatal("Process terminating because parent process has terminated");
			Environment.Exit(1);
		}
    }
}
