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

namespace Quokka.WinForms.Commands
{
	public class ActionCommand : ImageCommandBase
	{
		private Action _action;

		/// <summary>
		/// The action to perform when <see cref="Execute"/> is called.
		/// </summary>
		public Action Action
		{
			get { return _action; }
			set
			{
				if (_action == value) return;
				_action = value;
				RaisePropertyChanged("Action");
			}
		}

		/// <summary>
		/// Perform the action associated with this command
		/// </summary>
		public override void Execute()
		{
			if (Action != null)
			{
				Action();
			}
		}
	}
}