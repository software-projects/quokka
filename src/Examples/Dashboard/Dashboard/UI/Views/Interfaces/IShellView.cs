using System;

namespace Dashboard.UI.Views.Interfaces
{
	public interface IShellView
	{
		event EventHandler Logout;
		event EventHandler DoSomething;
		string Username { get; set; }
	}
}