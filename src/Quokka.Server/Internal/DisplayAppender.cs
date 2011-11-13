using Quokka.Diagnostics;
using Quokka.WinForms;
using log4net.Appender;
using log4net.Core;

namespace Quokka.Server.Internal
{
	public class DisplayAppender : AppenderSkeleton
	{
		private readonly EventLoggingDataSource<LoggingEvent> _dataSource;

		public DisplayAppender(EventLoggingDataSource<LoggingEvent> dataSource)
		{
			_dataSource = Verify.ArgumentNotNull(dataSource, "dataSource");
		}

		public FixFlags Fix { get; set; }

		protected override void Append(LoggingEvent loggingEvent)
		{
			loggingEvent.Fix = Fix;
			_dataSource.Add(loggingEvent);
		}
	}
}
