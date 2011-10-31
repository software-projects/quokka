using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    public class MessageQueueDataSource : VirtualDataSource<string, MessageQueueStatus>
    {
        protected override string GetId(MessageQueueStatus obj)
        {
            return obj.MessageQueueName;
        }
    }
}
