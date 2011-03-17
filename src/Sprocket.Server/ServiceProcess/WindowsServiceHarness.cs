using System.ComponentModel;
using System.ServiceProcess;
using Quokka.Diagnostics;

namespace Sprocket.Server.ServiceProcess
{
    [ToolboxItem(false)]
    public class WindowsServiceHarness : ServiceBase
    {
        private readonly WindowsService _windowsService;

        public WindowsServiceHarness(WindowsService windowsService)
        {
            _windowsService = Verify.ArgumentNotNull(windowsService, "windowsService");
        }

        protected override void OnStart(string[] args)
        {
            _windowsService.Start(args);
        }

        protected override void OnStop()
        {
            _windowsService.Stop();
        }
    }
}
