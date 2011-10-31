using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.UI.Commands;
using Quokka.WinForms.Commands;
using Sprocket.Manager.Tasks.Connect;

namespace Sprocket.Manager.Views.Connect
{
    [PerRequest(typeof(IConnectingView))]
    public partial class ConnectingView : UserControl, IConnectingView
    {
        private readonly UICommand _cancelCommand;

        public ConnectingView()
        {
            InitializeComponent();
            _cancelCommand = new UICommand(cancelButton);
        }

        public IUICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public string ServerName
        {
            set { connectingToLabel.Text = value; }
        }
    }
}