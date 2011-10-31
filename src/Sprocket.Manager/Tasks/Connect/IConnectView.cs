using Quokka.UI.Commands;

namespace Sprocket.Manager.Tasks.Connect
{
    public interface IConnectView
    {
        IUICommand ConnectCommand { get; }
        string Host { get; }
        int Port { get; }
    }
}