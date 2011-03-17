using System.Threading;
using Quokka.Events;
using Quokka.Sprocket;
using Quokka.Stomp.Server.Messages;
using Quokka.UI.Tasks;
using Sprocket.Manager.Events;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    public class StatusPresenter : Presenter<IStatusView>
    {
        public INavigateCommand DisconnectedCommand { get; set; }
        public IEventBroker EventBroker { get; set; }
        public SynchronizationContext SynchronizationContext { get; set; }
        public ISprocket Sprocket { get; set; }
        private readonly SessionDataSource _sessions = new SessionDataSource();
        private readonly MessageQueueDataSource _messageQueues = new MessageQueueDataSource();

        public override void InitializePresenter()
        {
            View.SessionDataSource = _sessions;
            View.MessageQueueDataSource = _messageQueues;
            Sprocket.CreateSubscriber<ServerStatusMessage>()
                .WithAction(HandleServerStatusMessage)
                .AddTo(Disposables);

            EventBroker.GetEvent<ConnectionStateChangedEvent>()
                .Subscribe(HandleConnectionStateChanged, ThreadOption.UIThread)
                .AddTo(Disposables);
        }

        private void HandleServerStatusMessage(ServerStatusMessage serverStatusMessage)
        {
            _sessions.ReplaceContents(serverStatusMessage.Sessions);
            _messageQueues.ReplaceContents(serverStatusMessage.MessageQueues);
        }

        private void HandleConnectionStateChanged(bool connected)
        {
            if (!connected)
            {
                DisconnectedCommand.Navigate();
            }
        }
    }
}
