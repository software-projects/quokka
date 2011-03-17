using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.UI.Commands;
using Quokka.UI.Tasks;
using Quokka.WinForms;
using Quokka.WinForms.Commands;
using Sprocket.Manager.Tasks.Connect;

namespace Sprocket.Manager.Views.Connect
{
    [ToolboxItem(false)]
    [PerRequest(typeof(IConnectedView))]
    public partial class ConnectedView : UserControl, IConnectedView
    {
        private readonly IUICommand _closeCommand;
        private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof (ConnectedView));
        private readonly IViewDeck _statusViewDeck;
        private readonly IViewDeck _logViewDeck;

        public ConnectedView()
        {
            InitializeComponent();
            _closeCommand = new UICommand(disconnectButton);
            _displaySettings.SaveSplitterWidth(splitContainer1);
            _statusViewDeck = new ViewDeck(splitContainer1.Panel1);
            _logViewDeck = new ViewDeck(splitContainer1.Panel2);
        }

        public IUICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public string ServerName
        {
            set { serverTitle.Text = value; }
        }

        public IViewDeck StatusViewDeck
        {
            get { return _statusViewDeck; }
        }

        public IViewDeck LogViewDeck
        {
            get { return _logViewDeck; }
        }
    }
}
