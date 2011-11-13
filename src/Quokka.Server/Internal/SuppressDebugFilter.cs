using log4net.Core;
using log4net.Filter;

namespace Quokka.Server.Internal
{
	internal class SuppressDebugFilter : FilterSkeleton
	{
		public string LoggerToMatch { get; set; }

		public override FilterDecision Decide(LoggingEvent loggingEvent)
		{
			if (!string.IsNullOrEmpty(LoggerToMatch)
				&& loggingEvent.LoggerName.StartsWith(LoggerToMatch))
			{
				if (loggingEvent.Level.Value < Level.Warn.Value)
				{
					return FilterDecision.Deny;
				}
			}

			return FilterDecision.Neutral;
		}
	}
}
