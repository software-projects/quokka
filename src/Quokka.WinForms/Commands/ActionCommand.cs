using System;

namespace Quokka.WinForms.Commands
{
	public class ActionCommand : UICommandBase
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