using System;
using System.IO;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Sprocket.Service.ServiceProcess;

namespace Sprocket.Service
{
    [WindowsService("Sprocket", 
        DisplayName="Sprocket", 
        Description="A service broker for distributed applications",
        StartMode = ServiceStartMode.Automatic)]
	public class SprocketService : WindowsService
    {
    	private ProcessSupervisor _processSupervisor;

		public override void Initialize()
		{
			var logDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			logDir = Path.Combine(logDir, "Sprocket");
			logDir = Path.Combine(logDir, "Log");
			Directory.CreateDirectory(logDir);
			GlobalContext.Properties["LogDir"] = logDir;

			var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sprocket.Server.log4net.config");

			var logFile = new FileInfo(logFilePath);
			XmlConfigurator.ConfigureAndWatch(logFile);

		}

		public override void Start(string[] args)
		{
			_processSupervisor = new ProcessSupervisor
			                     	{
			                     		ProgramName = "Sprocket.Server.exe"
			                     	};
			_processSupervisor.StartProcess();
		}

		public override void Stop()
		{
			_processSupervisor.StopProcess();
		}
	}
}
