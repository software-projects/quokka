using System;

namespace Dashboard.UI.Views.Interfaces
{
	public interface IShellView
	{
		event EventHandler Logout;
		string Username { get; set; }
	}
}