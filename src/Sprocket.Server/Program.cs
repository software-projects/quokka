using Sprocket.Server.ServiceProcess;

namespace Sprocket.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WindowsService.Run<SprocketServer>(args);
        }
    }
}