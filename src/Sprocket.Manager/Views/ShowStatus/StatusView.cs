using System.ComponentModel;
using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.Stomp.Server.Messages;
using Quokka.WinForms;
using Sprocket.Manager.Tasks.ShowStatus;

namespace Sprocket.Manager.Views.ShowStatus
{
    [ToolboxItem(false)]
    [PerRequest(typeof(IStatusView))]
    public partial class StatusView : UserControl, IStatusView
    {
        private readonly VirtualDataGridViewAdapter<SessionStatus> _sessionGrid;
        private readonly VirtualDataGridViewAdapter<MessageQueueStatus> _messageGrid;
        private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof(StatusView));
        public StatusView()
        {
            InitializeComponent();
            sessionDataGridView.AllowUserToResizeRows = false;
            sessionDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            messageQueueDataGridView.AllowUserToResizeRows = false;
            messageQueueDataGridView.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            _sessionGrid = new VirtualDataGridViewAdapter<SessionStatus>(sessionDataGridView);
            _sessionGrid
                .DefineCellValue(sessionClientIdColumn, s => s.ClientId)
                .DefineCellValue(sessionIdColumn, s => s.SessionId)
                .DefineCellValue(sessionConnectedColumn, s => s.Connected)
                .DefineCellValue(sessionSubscriptionCountColumn, s => s.Subscriptions.Count)
                .SetDefaultSortOrder(sessionClientIdColumn, sessionIdColumn, sessionConnectedColumn,
                                     sessionSubscriptionCountColumn)
                .SortBy(sessionClientIdColumn)
                .WithDisplaySettings(_displaySettings);

            _messageGrid =
                new VirtualDataGridViewAdapter<MessageQueueStatus>(messageQueueDataGridView);
            _messageGrid
                .DefineCellValue(messageQueueNameColumn, m => m.MessageQueueName)
                .DefineCellValue(messageQueueSubscriptionCountColumn, m => m.SubscriptionCount)
                .DefineCellValue(messageQueuePendingMessageCountColumn, m => m.PendingMessageCount)
                .DefineCellValue(messageQueueTotalMessagesCountColumn, m => m.TotalMessageCount)
                .SetDefaultSortOrder(messageQueueNameColumn, messageQueueSubscriptionCountColumn,
                                     messageQueuePendingMessageCountColumn, messageQueueTotalMessagesCountColumn)
                .SortBy(messageQueueNameColumn)
                .WithDisplaySettings(_displaySettings);
            _displaySettings.SaveSplitterWidth(splitContainer);
        }

        public VirtualDataSource<string, SessionStatus> SessionDataSource
        {
            set { _sessionGrid.Init(value); }
        }

        public VirtualDataSource<string, MessageQueueStatus> MessageQueueDataSource
        {
            set { _messageGrid.Init(value); }
        }
    }
}
