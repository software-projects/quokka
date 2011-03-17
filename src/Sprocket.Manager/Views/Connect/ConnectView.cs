using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.UI.Commands;
using Quokka.WinForms.Commands;
using Sprocket.Manager.Tasks.Connect;

namespace Sprocket.Manager.Views.Connect
{
    [PerRequest(typeof(IConnectView))]
    public partial class ConnectView : UserControl, IConnectView
    {
        private readonly IUICommand _connectCommand;

        public ConnectView()
        {
            InitializeComponent();
            _connectCommand = new UICommand(connectButton);
        }

        public IUICommand ConnectCommand
        {
            get { return _connectCommand; }
        }

        public string Host
        {
            get { return hostTextBox.Text; }
        }

        public int Port
        {
            get
            {
                int port;
                if (int.TryParse(portTextBox.Text, out port))
                {
                    return port;
                }

                port = 6867;
                portTextBox.Text = port.ToString();
                return port;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                connectButton.PerformClick();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}