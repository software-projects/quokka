using ComponentFactory.Krypton.Toolkit;
using Quokka.Events;
using Quokka.Sprocket;
using Quokka.UI.Tasks;
using Quokka.WinForms;
using Sprocket.Manager.Events;

namespace Sprocket.Manager
{
    public partial class MainForm : KryptonForm
    {
        private readonly ViewDeck _viewDeck;
        public IEventBroker EventBroker { get; set; }
        public ISprocket Sprocket { get; set; }
        private readonly DisplaySettings _displaySettings;

        public MainForm()
        {
            InitializeComponent();
            _viewDeck = new ViewDeck(mainPanel);
            _displaySettings = new DisplaySettings(this);
        }

        public IViewDeck ViewDeck
        {
            get { return _viewDeck; }
        }

        private void FormLoad(object sender, System.EventArgs e)
        {
            if (Sprocket == null)
            {
                serverEndPointLabel.Text = "No Sprocket!";
                connectionStatus.Text = "Not connected";
            }
            else
            {
                Sprocket.ConnectedChanged += SprocketConnectedChanged;
            }
        }

        void SprocketConnectedChanged(object sender, System.EventArgs e)
        {
            if (Sprocket == null)
            {
                serverEndPointLabel.Text = "No Sprocket!";
                connectionStatus.Text = "Disconnected";
            }
            else
            {
                serverEndPointLabel.Text = Sprocket.ServerUrl == null ? string.Empty : Sprocket.ServerUrl.ToString();
                connectionStatus.Text = Sprocket.Connected ? "Connected" : "Not connected";

                EventBroker.GetEvent<ConnectionStateChangedEvent>()
                    .Publish(Sprocket.Connected);
            }
        }
    }
}