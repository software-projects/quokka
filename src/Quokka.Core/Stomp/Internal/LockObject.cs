using System;
using System.Collections.Generic;
using System.Threading;

namespace Quokka.Stomp.Internal
{
	public class LockObject
	{
		private int _count;
		private List<Action> _actions = new List<Action>();
		private readonly IDisposable _disposable;

		public LockObject()
		{
			_disposable = new DisposableAction(Unlock);
		}

		public IDisposable Lock()
		{
			Monitor.Enter(this);
			++_count;

			return _disposable;
		}

		public void AfterUnlock(Action action)
		{
			if (action != null)
			{
				if (_actions == null)
				{
					_actions = new List<Action>();
				}
				_actions.Add(action);
			}
		}

		private void Unlock()
		{
			List<Action> actions = null;
			--_count;
			if (_count == 0)
			{
				actions = _actions;
				_actions = null;
			}
			Monitor.Exit(this);

			if (actions == null)
			{
				return;
			}

			foreach (var action in actions)
			{
				action();
			}
		}
	}
}