using System;
using Quokka.Events;
using Quokka.Sprocket;
using Quokka.UI.Messages;
using Quokka.UI.Tasks;

namespace Sprocket.Manager.Tasks.Connect
{
    public class ConnectPresenter : Presenter<IConnectView>
    {
        public UIMessageBox MessageBox { get; set; }
        public IEventBroker EventBroker { get; set; }
        public INavigateCommand ConnectCommand { get; set; }
        public ISprocket Sprocket { get; set; }

        public override void InitializePresenter()
        {
            View.ConnectCommand.Execute += ConnectCommandExecute;
        }

        private void ConnectCommandExecute(object sender, EventArgs e)
        {
            var urlText = string.Format("tcp://{0}:{1}", View.Host, View.Port);
            var url = new Uri(urlText);
            Sprocket.Open(url);
            ConnectCommand.Navigate();
        }
    }
}