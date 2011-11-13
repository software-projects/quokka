using System;
using System.Text.RegularExpressions;

namespace Quokka.Config.Internal
{
	public static class TimeSpanExtensions
	{
		public static string ToReadableString(this TimeSpan @this)
		{
			// For zero timespan, choose seconds as the unit
			if (@this == TimeSpan.Zero)
			{
				return "0s";
			}

			if (@this.Milliseconds != 0)
			{
				return @this.TotalMilliseconds + "ms";
			}

			if (@this.Seconds != 0)
			{
				return @this.TotalSeconds + "s";
			}

			if (@this.Minutes != 0)
			{
				return @this.TotalMinutes + "m";
			}

			if (@this.Hours != 0)
			{
				return @this.TotalHours + "h";
			}

			return @this.TotalDays + "d";
		}

		public static bool TryParse(string s, out TimeSpan result)
		{
			var regex = new Regex(@"^(\s*(\d+)\s*([a-z]+)\s*,?\s*)+$", RegexOptions.IgnoreCase);
			var match = regex.Match(s);
			if (!match.Success)
			{
				// not a match, so try the default TryParse
				return TimeSpan.TryParse(s, out result);
			}

			TimeSpan total = TimeSpan.Zero;

			// Group 0 = entire expression
			// Group 1 = outer parenthesis expression (repeated digits+units)
			// Group 2 = digits
			// Group 3 = units
			var digitsGroup = match.Groups[2];
			var unitsGroup = match.Groups[3];
			var captureCount = digitsGroup.Captures.Count;

			for (int captureNum = 0; captureNum < captureCount; ++captureNum)
			{

				var value = int.Parse(digitsGroup.Captures[captureNum].Value);
				var units = unitsGroup.Captures[captureNum].Value;

				switch (units)
				{
					case "d":
					case "day":
					case "days":
						total += TimeSpan.FromDays(value);
						break;
					case "h":
					case "hour":
					case "hours":
					case "hr":
					case "hrs":
						total += TimeSpan.FromHours(value);
						break;
					case "m":
					case "min":
					case "mins":
					case "minute":
					case "minutes":
						total += TimeSpan.FromMinutes(value);
						break;
					case "s":
					case "sec":
					case "secs":
					case "second":
					case "seconds":
						total += TimeSpan.FromSeconds(value);
						break;
					case "ms":
					case "msec":
					case "msecs":
					case "millisecond":
					case "milliseconds":
						total += TimeSpan.FromMilliseconds(value);
						break;

					default:
						result = default(TimeSpan);
						return false;
				}
			}
			result = total;
			return true;
		}
	}
}
