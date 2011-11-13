using System;
using NUnit.Framework;
using Quokka.Config.Internal;

namespace Quokka.Config
{
	[TestFixture]
	public class TimeSpanExtensionsTests
	{
		[Test]
		public void ToReadableString()
		{
			var timeSpan = new TimeSpan(0, 0, 0, 0, 25);
			Assert.AreEqual("25ms", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(0, 0, 1, 0, 25);
			Assert.AreEqual("60025ms", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(1, 0, 0, 1, 0);
			Assert.AreEqual("86401s", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(0, 0, 2, 0, 0);
			Assert.AreEqual("2m", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(1, 1, 2, 0, 0);
			Assert.AreEqual("1502m", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(1, 1, 0, 0, 0);
			Assert.AreEqual("25h", timeSpan.ToReadableString());

			timeSpan = new TimeSpan(1, 0, 0, 0, 0);
			Assert.AreEqual("1d", timeSpan.ToReadableString());
		}

		[Test]
		public void ParseReadableString()
		{
			TimeSpan timeSpan;

			Assert.IsTrue(TimeSpanExtensions.TryParse("25ms", out timeSpan));
			Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 25), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1s 500ms", out timeSpan));
			Assert.AreEqual(new TimeSpan(0, 0, 0, 1, 500), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1h 30m", out timeSpan));
			Assert.AreEqual(new TimeSpan(0, 90, 0), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("30m1h", out timeSpan));
			Assert.AreEqual(new TimeSpan(0, 90, 0), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1d2h3m4s5ms", out timeSpan));
			Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("50ms 40s 30m 20h 10d", out timeSpan));
			Assert.AreEqual(new TimeSpan(10, 20, 30, 40, 50), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1 day 2 hours 3 minutes 4 seconds 5milliseconds", out timeSpan));
			Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("4 days, 2 hours, 3 minutes, 4 seconds, 5 milliseconds", out timeSpan));
			Assert.AreEqual(new TimeSpan(4, 2, 3, 4, 5), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("4 days, 2 hrs, 3 mins, 4 secs, 5 msecs", out timeSpan));
			Assert.AreEqual(new TimeSpan(4, 2, 3, 4, 5), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1 day, 1 hour, 1 minute, 1 second, 1 millisecond", out timeSpan));
			Assert.AreEqual(new TimeSpan(1, 1, 1, 1, 1), timeSpan);

			Assert.IsTrue(TimeSpanExtensions.TryParse("1 day, 1 hr, 1 min, 1 sec, 1 msec", out timeSpan));
			Assert.AreEqual(new TimeSpan(1, 1, 1, 1, 1), timeSpan);
		}
	}
}
