using System;
using Quokka.Sprocket;
using Quokka.Stomp.Server.Messages;
using Quokka.UI.Tasks;
using Quokka.WinForms;

namespace Sprocket.Manager.Tasks.ShowLog
{
    public class MessageLogPresenter : Presenter<IMessageLogView>
    {
        public ISprocket Sprocket { get; set; }
        private EventLoggingDataSource<MessageLogMessage> _messages = new EventLoggingDataSource<MessageLogMessage>(); 

        public override void InitializePresenter()
        {
            View.DataSource = _messages;
            Sprocket.CreateSubscriber<MessageLogMessage>()
                .WithAction(HandleMessageLogMessage)
                .AddTo(Disposables);
        }

        private void HandleMessageLogMessage(MessageLogMessage mlm)
        {
            _messages.Add(mlm);
        }
    }
}
