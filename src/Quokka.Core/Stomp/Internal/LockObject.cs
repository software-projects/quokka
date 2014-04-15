#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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