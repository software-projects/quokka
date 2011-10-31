using Quokka.UI.Commands;
using Quokka.UI.Tasks;

namespace Sprocket.Manager.Tasks.Connect
{
    public interface IConnectedView
    {
        string ServerName { set; }
        IUICommand CloseCommand { get; }
        IViewDeck StatusViewDeck { get; }
        IViewDeck LogViewDeck { get; }
    }
}
