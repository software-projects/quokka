using System;

namespace Sprocket.Server.SupervisedProcess
{
    public class MonitoredService : IDisposable
    {
        public bool IsConsoleMode { get; private set; }


        public virtual void Initialize()
        {
        }

        public virtual void Start(string[] args)
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Dispose()
        {
        }

        public static void Run<T>(string[] args) where T : MonitoredService, new()
        {
			if (!Environment.UserInteractive
				&& Environment.GetEnvironmentVariable(MonitoredProcessHarness.StopEventNameEnvironmentVariable) != null)
			{
				// We are a monitored process.
				using (var service = new T { IsConsoleMode = false })
				{
					service.Initialize();
					var harness = new MonitoredProcessHarness(service);
					harness.Run(args);
				}
			}
			else
			{
				using (var service = new T {IsConsoleMode = true})
				{
					service.Initialize();
					var harness = new ConsoleHarness(service);
					harness.Run(args);
				}
			}
        }
    }
}