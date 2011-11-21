using System;

namespace Quokka.Services
{
	public class SystemClock : IClock
	{
		public DateTimeOffset Now
		{
			get { return DateTimeOffset.Now; }
		}
	}
}
