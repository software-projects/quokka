using System;
using System.ServiceProcess;

namespace Sprocket.Service.ServiceProcess
{
    public class WindowsService : IDisposable
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

        public static void Run<T>(string[] args) where T : WindowsService, new()
        {
            if (!Environment.UserInteractive)
            {
                // We are a windows service.
                var windowsService = new T {IsConsoleMode = false};
                windowsService.Initialize();
                ServiceBase.Run(new WindowsServiceHarness(windowsService));
                return;
            }

            // We are running interactively from the command line.
            var commandLine = new CommandLine(args);

            if (commandLine.Error)
            {
                commandLine.WriteErrorMessage(Console.Out);
                Environment.ExitCode = 1;
                return;
            }

            if (commandLine.ShowHelp)
            {
                commandLine.WriteUsage(Console.Out);
                Environment.ExitCode = 1;
                return;
            }


            // if install was a command line flag, then run the installer at runtime.
            if (commandLine.Install)
            {
                WindowsServiceInstaller.RuntimeInstall<T>();
            }
  
                // if uninstall was a command line flag, run uninstaller at runtime.
            else if (commandLine.Uninstall)
            {
                WindowsServiceInstaller.RuntimeUnInstall<T>();
            }
  
                // otherwise, fire up the service as either console or windows service based on UserInteractive property.
            else if (commandLine.ConsoleMode)
            {
                var windowsService = new T {IsConsoleMode = true};
                windowsService.Initialize();

                // TODO: here we pass empty args to the implementation. If this were to become a more
                // generic library we might have some sort of arg passing mechanism.
                ConsoleHarness.Run(windowsService, new string[0]);
            }
        }
    }
}