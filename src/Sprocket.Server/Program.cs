using Sprocket.Server.SupervisedProcess;

namespace Sprocket.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MonitoredService.Run<SprocketServer>(args);
        }
    }
}