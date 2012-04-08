using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Event arguments with a <see cref="UITask"/>.
	/// </summary>
	public class UITaskEventArgs : EventArgs
	{
		public UITask Task { get; private set; }

		public UITaskEventArgs(UITask task)
		{
			Task = task;
		}
	}
}