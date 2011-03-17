using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    [PerRequest(typeof(ShowStatusTask))]
    public class ShowStatusTask : UITask
    {
        protected override void CreateNodes()
        {
            var notConnectedNode = CreateNode();
            var statusNode = CreateNode();

            notConnectedNode
                .SetPresenter<NotConnectedPresenter>()
                .NavigateTo(p => p.ConnectedCommand, statusNode);

            statusNode
                .SetPresenter<StatusPresenter>()
                .NavigateTo(p => p.DisconnectedCommand, notConnectedNode);
        }
    }
}
