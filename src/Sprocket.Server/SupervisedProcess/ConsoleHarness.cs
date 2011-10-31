using System;
using System.Threading;
using Quokka.Diagnostics;

namespace Sprocket.Server.SupervisedProcess
{
    public class ConsoleHarness
    {
        private readonly MonitoredService _serviceImplementation;
        private readonly ManualResetEvent _stopEvent;

		public ConsoleHarness(MonitoredService serviceImplementation)
		{
			_serviceImplementation = Verify.ArgumentNotNull(serviceImplementation, "serviceImplementation");
			_stopEvent = new ManualResetEvent(false);
		}

		public void Run(string[] args)
		{
			if (args == null)
			{
				args = new string[0];
			}
			Console.CancelKeyPress += ConsoleCancelKeyPress;
			_serviceImplementation.Start(args);
			_stopEvent.WaitOne();
			_serviceImplementation.Stop();
		}

    	private void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _stopEvent.Set();
        }

         // Helper method to write a message to the console at the given foreground color.
         public static void WriteToConsole(ConsoleColor foregroundColor, string format,
             params object[] formatArguments)
         {
             ConsoleColor originalColor = Console.ForegroundColor;
             Console.ForegroundColor = foregroundColor;
  
             Console.WriteLine(format, formatArguments);
             Console.Out.Flush();
  
             Console.ForegroundColor = originalColor;
         }
    }
}
