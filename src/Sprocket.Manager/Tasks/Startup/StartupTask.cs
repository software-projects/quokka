using Quokka.UI.Tasks;
using Sprocket.Manager.Tasks.Connect;

namespace Sprocket.Manager.Tasks.Startup
{
    public class StartupTask : UITask
    {
        protected override void CreateNodes()
        {
            var node = CreateNode();

            node.SetNestedTask<ConnectTask>();
        }
    }
}