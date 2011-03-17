using System;
using Quokka.Events;
using Quokka.Sprocket;
using Quokka.UI.Tasks;
using Sprocket.Manager.Events;
using Sprocket.Manager.Tasks.ShowLog;
using Sprocket.Manager.Tasks.ShowStatus;
using Sprocket.Manager.Views.ShowLog;

namespace Sprocket.Manager.Tasks.Connect
{
    public class ConnectedPresenter : Presenter<IConnectedView>
    {
        public ISprocket Sprocket { get; set; }
        public IEventBroker EventBroker { get; set; }
        public INavigateCommand LostConnectionCommand { get; set; }
        public INavigateCommand CloseCommand { get; set; }
        public ShowStatusTask ShowStatusTask { get; set; }
        public ShowLogTask ShowLogTask { get; set; }

        public override void InitializePresenter()
        {
            EventBroker.GetEvent<ConnectionStateChangedEvent>()
                .Subscribe(HandleConnectionStateChanged, ThreadOption.UIThread)
                .AddTo(Disposables);
            View.CloseCommand.Execute += CloseCommandExecute;
            View.ServerName = Sprocket.ServerUrl.ToString();
            ShowStatusTask.Start(View.StatusViewDeck);
            ShowLogTask.Start(View.LogViewDeck);
        }

        void CloseCommandExecute(object sender, EventArgs e)
        {
            Sprocket.Close();
            CloseCommand.Navigate();
        }

        private void HandleConnectionStateChanged(bool connected)
        {
            if (!connected)
            {
                LostConnectionCommand.Navigate();
            }
        }
    }
}
