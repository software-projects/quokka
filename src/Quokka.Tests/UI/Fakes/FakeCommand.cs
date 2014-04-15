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
using System.ComponentModel;
using Quokka.UI.Commands;

namespace Quokka.UI.Fakes
{
	public class FakeCommand : IUICommand
	{
		public event EventHandler Execute;
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _checked;

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					RaisePropertyChanged("Checked");
				}
			}
		}

		public bool CanCheck
		{
			get { return true; }
		}

		private bool _enabled;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					RaisePropertyChanged("Enabled");
				}
			}
		}

		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					RaisePropertyChanged("Text");
				}
			}
		}

		public void PerformExecute()
		{
			if (Execute != null)
			{
				Execute(this, EventArgs.Empty);
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}