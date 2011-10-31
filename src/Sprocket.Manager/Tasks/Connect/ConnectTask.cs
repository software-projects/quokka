using Quokka.Events;
using Quokka.ServiceLocation;
using Quokka.Stomp;
using Quokka.UI.Tasks;
using Sprocket.Manager.Events;

namespace Sprocket.Manager.Tasks.Connect
{
    [PerRequest(typeof(ConnectTask))]
    public class ConnectTask : UITask
    {
        protected override void CreateNodes()
        {
            var connectNode = CreateNode();
            var connectingNode = CreateNode();
            var connectedNode = CreateNode();

            connectNode
                .SetPresenter<ConnectPresenter>()
                .NavigateTo(p => p.ConnectCommand, connectingNode);

            connectingNode
                .SetPresenter<ConnectingPresenter>()
                .NavigateTo(p => p.CancelCommand, connectNode)
                .NavigateTo(p => p.ConnectedCommand, connectedNode);

            connectedNode
                .SetPresenter<ConnectedPresenter>()
                .NavigateTo(p => p.LostConnectionCommand, connectNode)
                .NavigateTo(p => p.CloseCommand, connectNode);

        }
    }
}