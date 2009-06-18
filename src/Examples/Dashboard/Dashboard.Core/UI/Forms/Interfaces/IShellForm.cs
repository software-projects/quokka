using System;

namespace Dashboard.UI.Forms.Interfaces
{
	public interface IShellForm
	{
		event EventHandler Logout;
		string Username { get; set; }
	}
}