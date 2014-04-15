using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Event arguments with a <see cref="UITask"/>.
	/// </summary>
	public class UITaskEventArgs : EventArgs
	{
		public IUITask Task { get; private set; }

		public UITaskEventArgs(IUITask task)
		{
			Task = task;
		}
	}
}