using System;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Quokka.Spikes
{
	[TestFixture]
	public class MiscSpikeTests
	{
		//[Test]
		//[Ignore("Simple spike does not work")]
		public void TimeSpan_parse_formats()
		{
			var timeSpan = TimeSpan.Parse("1h 30m");

		}

		[Test]
		public void Boolean_parse_formats()
		{
			bool result;

			// Values that could be interpreted as true, but are not parsed
			Assert.IsFalse(bool.TryParse("y", out result));
			Assert.IsFalse(bool.TryParse("yes", out result));
			Assert.IsFalse(bool.TryParse("1", out result));
			Assert.IsFalse(bool.TryParse("t", out result));
		}
	}
}
