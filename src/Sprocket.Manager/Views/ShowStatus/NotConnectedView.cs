using System.ComponentModel;
using System.Windows.Forms;
using Quokka.ServiceLocation;
using Sprocket.Manager.Tasks.ShowStatus;

namespace Sprocket.Manager.Views.ShowStatus
{
    [ToolboxItem(false)]
    [PerRequest(typeof(INotConnectedView))]
    public partial class NotConnectedView : UserControl, INotConnectedView
    {
        public NotConnectedView()
        {
            InitializeComponent();
        }
    }
}
