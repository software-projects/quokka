using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

namespace Sprocket.Manager.Tasks.ShowLog
{
    [PerRequest(typeof(ShowLogTask))]
    public class ShowLogTask : UITask
    {
        protected override void CreateNodes()
        {
            var node = CreateNode();

            node.SetPresenter<MessageLogPresenter>();
        }
    }
}
