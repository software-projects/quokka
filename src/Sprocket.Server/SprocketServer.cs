using System;
using System.Configuration;
using System.IO;
using System.Net;
using log4net;
using log4net.Config;
using Quokka.Stomp;
using Sprocket.Server.SupervisedProcess;

namespace Sprocket.Server
{
    public class SprocketServer : MonitoredService
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
            	Log.Info("Server started, listening on port " + portNumber);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to start server: " + ex.Message, ex);
                throw;
            }
        }

        public override void Stop()
        {
        	System.Threading.Thread.Sleep(20000);
            _stompServer.Dispose();
            Log.Info("Server stopped");
        }
    }
}