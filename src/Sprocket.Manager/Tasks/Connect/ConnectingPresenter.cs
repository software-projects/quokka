using System;
using Quokka.Events;
using Quokka.Sprocket;
using Quokka.UI.Tasks;
using Sprocket.Manager.Events;

namespace Sprocket.Manager.Tasks.Connect
{
    public class ConnectingPresenter : Presenter<IConnectingView>
    {
        public IEventBroker EventBroker { get; set; }
        public ISprocket Sprocket { get; set; }
        public INavigateCommand ConnectedCommand { get; set; }
        public INavigateCommand CancelCommand { get; set; }

        public override void InitializePresenter()
        {
            View.CancelCommand.Execute += CancelCommandExecute;
            View.ServerName = Sprocket.ServerUrl.ToString();
            EventBroker.GetEvent<ConnectionStateChangedEvent>()
                .Subscribe(HandleConnectionStateChanged, ThreadOption.UIThread)
                .AddTo(Disposables);

            if (Sprocket.Connected)
            {
                ConnectedCommand.Navigate();
            }
        }

        private void CancelCommandExecute(object sender, EventArgs e)
        {
            Sprocket.Close();
            CancelCommand.Navigate();
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
