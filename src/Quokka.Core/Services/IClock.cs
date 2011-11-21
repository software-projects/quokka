using System;

namespace Quokka.Services
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
