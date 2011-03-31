using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace Sprocket.Service.ServiceProcess
{
    // A generic windows service installer
    [RunInstaller(true)]
    [ToolboxItem(false)]
    public class WindowsServiceInstaller : Installer
    {
        // Gets or sets the type of the windows service to install.
        public WindowsServiceAttribute Configuration { get; set; }

        // Creates a blank windows service installer with configuration in ServiceImplementation
        public WindowsServiceInstaller() : this(typeof (WindowsService))
        {
        }

        // Creates a windows service installer using the type specified.
        public WindowsServiceInstaller(Type windowsServiceType)
        {
            if (!typeof (WindowsService).IsAssignableFrom(windowsServiceType))
            {
                throw new ArgumentException("Type to install must inherit from WindowsService.",
                                            "windowsServiceType");
            }

            var attribute = windowsServiceType.GetAttribute<WindowsServiceAttribute>();

            if (attribute == null)
            {
                throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.",
                                            "windowsServiceType");
            }

            Configuration = attribute;
        }

        // Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
        public static void RuntimeInstall<T>()
            where T : WindowsService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            try
            {
                using (var ti = new TransactedInstaller())
                {
                    ti.Installers.Add(new WindowsServiceInstaller(typeof (T)));
                    ti.Context = new InstallContext(null, new[] {path});
                    ti.Install(new Hashtable());
                }
            }
            catch (InvalidOperationException ex)
            {
                HandleException(ex);
            }
            catch (InstallException ex)
            {
                HandleException(ex);
            }
        }

        private static void HandleException(Exception ex)
        {
            var message = ex.Message;
            ConsoleHarness.WriteToConsole(ConsoleColor.Red, message);
            var innerEx = ex.InnerException;
            if (innerEx != null)
            {
                message = innerEx.GetType().FullName + ": " + innerEx.Message;
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, message);
            }
            Environment.ExitCode = 1;
        }

        // Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
        public static void RuntimeUnInstall<T>(params Installer[] otherInstallers)
            where T : WindowsService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            try
            {
                using (var ti = new TransactedInstaller())
                {
                    ti.Installers.Add(new WindowsServiceInstaller(typeof (T)));
                    ti.Context = new InstallContext(null, new[] {path});
                    ti.Uninstall(null);
                }
            }
            catch (InstallException ex)
            {
                HandleException(ex);
            }
        }

        // Installer class, to use run InstallUtil against this .exe
        public override void Install(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Installing service {0}.", Configuration.Name);

            // install the service 
            ConfigureInstallers();
            base.Install(savedState);
        }

        // Removes the counters, then calls the base uninstall.
        public override void Uninstall(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "UnInstalling service {0}.", Configuration.Name);

            // load the assembly file name and the config
            ConfigureInstallers();
            base.Uninstall(savedState);
        }

        // Method to configure the installers
        private void ConfigureInstallers()
        {
            // load the assembly file name and the config
            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());
        }

        // Helper method to configure a process installer for this windows service
        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            // if a user name is not provided, will run under local service acct
            if (string.IsNullOrEmpty(Configuration.UserName))
            {
                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;
            }
            else
            {
                // otherwise, runs under the specified user authority
                result.Account = ServiceAccount.User;
                result.Username = Configuration.UserName;
                result.Password = Configuration.Password;
            }

            return result;
        }

        // Helper method to configure a service installer for this windows service
        private ServiceInstaller ConfigureServiceInstaller()
        {
            // create and config a service installer
            var result = new ServiceInstaller
                             {
                                 ServiceName = Configuration.Name,
                                 DisplayName = Configuration.DisplayName,
                                 Description = Configuration.Description,
                                 StartType = Configuration.StartMode,
                             };

            return result;
        }
    }
}