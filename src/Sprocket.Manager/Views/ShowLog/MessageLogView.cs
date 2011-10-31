using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;
using Sprocket.Manager.Tasks.ShowLog;

namespace Sprocket.Manager.Views.ShowLog
{
    [PerRequest(typeof(IMessageLogView))]
    public partial class MessageLogView : UserControl, IMessageLogView
    {
        private VirtualDataGridViewAdapter<MessageLogMessage> _dataGrid;
        private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof (MessageLogView));

        public MessageLogView()
        {
            InitializeComponent();
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            _dataGrid = new VirtualDataGridViewAdapter<MessageLogMessage>(dataGridView);
            _dataGrid
                .DefineCellValue(dateTimeColumn, m => m.SentAt)
                .DefineCellValue(destinationColumn, m => m.Destination)
                .DefineCellValue(contentLengthColumn, m => m.ContentLength)
                .WithDisplaySettings(_displaySettings);
        }

        public IVirtualDataSource<MessageLogMessage> DataSource
        {
            set { _dataGrid.Init(value); }
        }
    }
}
