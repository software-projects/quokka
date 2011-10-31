using Sprocket.Service.ServiceProcess;

namespace Sprocket.Service
{
	class Program
	{
		static void Main(string[] args)
		{
			WindowsService.Run<SprocketService>(args);
		}
	}
}
