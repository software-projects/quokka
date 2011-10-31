using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;
using Quokka.WinForms;
using Sprocket.Manager.Tasks;
using Sprocket.Manager.Tasks.Connect;
using Sprocket.Manager.Tasks.ShowStatus;

namespace Sprocket.Manager.Views
{
    public partial class LayoutView : UserControl
    {
        private readonly ViewDeck _bottomViewDeck;
        private readonly ViewDeck _leftViewDeck;
        private readonly ViewDeck _rightViewDeck;

        public ConnectTask ConnectTask { get; set; }
        public ShowStatusTask ShowStatusTask { get; set; }

        public LayoutView()
        {
            InitializeComponent();
            _leftViewDeck = new ViewDeck(leftPanel);
            _rightViewDeck = new ViewDeck(rightPanel);
            _bottomViewDeck = new ViewDeck(bottomPanel);
        }

        public IViewDeck LeftViewDeck
        {
            get { return _leftViewDeck; }
        }

        public IViewDeck RightViewDeck
        {
            get { return _rightViewDeck; }
        }

        public IViewDeck BottomViewDeck
        {
            get { return _bottomViewDeck; }
        }

        private void ViewLoad(object sender, System.EventArgs e)
        {
            ConnectTask.Start(LeftViewDeck);
            ShowStatusTask.Start(RightViewDeck);

        }
    }
}