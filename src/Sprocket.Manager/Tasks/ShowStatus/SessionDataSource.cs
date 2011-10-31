using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    public class SessionDataSource : VirtualDataSource<string, SessionStatus>
    {
        protected override string GetId(SessionStatus obj)
        {
            return obj.SessionId;
        }
    }
}
