using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;

namespace Sprocket.Manager.Tasks.ShowLog
{
    public interface IMessageLogView
    {
        IVirtualDataSource<MessageLogMessage> DataSource { set; }
    }
}
