using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Quokka.Stomp;
using Sprocket.Server.ServiceProcess;

namespace Sprocket.Server
{
    [WindowsService("Sprocket.Server", 
        DisplayName="Sprocket Server", 
        Description="A simple Service Broker",
        StartMode = ServiceStartMode.Automatic)]
    public class SprocketServer : WindowsService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SprocketServer));
        private StompServer _stompServer;

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
            try
            {
                _stompServer = new StompServer();
                int portNumber = int.Parse(ConfigurationManager.AppSettings["listenPort"]);
                var ipv4EndPoint = new IPEndPoint(IPAddress.Any, portNumber);
                _stompServer.ListenOn(ipv4EndPoint);
				if (System.Net.Sockets.Socket.OSSupportsIPv6)
				{
					var ipv6EndPoint = new IPEndPoint(IPAddress.IPv6Any, portNumber);
					_stompServer.ListenOn(ipv6EndPoint);
				}
            	Log.Info("Service started, listening on port " + portNumber);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to start service: " + ex.Message, ex);
                throw;
            }
        }

        public override void Stop()
        {
            _stompServer.Dispose();
            Log.Info("Service stopped");
        }
    }
}