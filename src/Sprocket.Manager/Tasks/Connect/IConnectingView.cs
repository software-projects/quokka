using Quokka.UI.Commands;

namespace Sprocket.Manager.Tasks.Connect
{
    public interface IConnectingView
    {
        string ServerName { set; }
        IUICommand CancelCommand { get; }
    }
}
