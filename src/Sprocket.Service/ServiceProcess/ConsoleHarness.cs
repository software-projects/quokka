using System;
using System.Threading;
using Quokka.Diagnostics;

namespace Sprocket.Service.ServiceProcess
{
    public static class ConsoleHarness
    {
        private static WindowsService _serviceImplementation;
        private static ManualResetEvent _stopEvent;

        public static void Run(WindowsService serviceImplementation, string[] args)
        {
            _serviceImplementation = Verify.ArgumentNotNull(serviceImplementation, "serviceImplementation");
            if (args == null)
            {
                args = new string[0];
            }
            Console.CancelKeyPress += ConsoleCancelKeyPress;
            using (_stopEvent = new ManualResetEvent(false))
            {
                _serviceImplementation.Start(args);
                _stopEvent.WaitOne();
            }
            _serviceImplementation.Stop();
        }

        private static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
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
