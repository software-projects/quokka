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