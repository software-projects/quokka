using Quokka.Events;
using Quokka.Sprocket;
using Quokka.UI.Tasks;
using Sprocket.Manager.Events;

namespace Sprocket.Manager.Tasks.ShowStatus
{
    public class NotConnectedPresenter : Presenter<INotConnectedView>
    {
        public INavigateCommand ConnectedCommand { get; set; }
        public IEventBroker EventBroker { get; set; }
        public ISprocket Sprocket { get; set; }

        public override void InitializePresenter()
        {
            EventBroker.GetEvent<ConnectionStateChangedEvent>()
                .Subscribe(HandleConnectionStateChanged, ThreadOption.UIThread)
                .AddTo(Disposables);

            if (Sprocket.Connected)
            {
                ConnectedCommand.Navigate();
            }
        }

        private void HandleConnectionStateChanged(bool connected)
        {
            if (connected)
            {
                ConnectedCommand.Navigate();
            }
        }
    }
}
