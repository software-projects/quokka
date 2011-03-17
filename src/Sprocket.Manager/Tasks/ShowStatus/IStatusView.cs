using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    public interface IStatusView
    {
        VirtualDataSource<string, SessionStatus> SessionDataSource { set; }
        VirtualDataSource<string, MessageQueueStatus> MessageQueueDataSource { set; }
    }
}
