using System;
using Quokka.Diagnostics;

namespace Quokka.Server.Internal
{
	public class DisposableAction : IDisposable
	{
		private readonly Action _action;

		public DisposableAction(Action action)
		{
			_action = Verify.ArgumentNotNull(action, "action");
		}

		public void Dispose()
		{
			_action();
		}
	}
}