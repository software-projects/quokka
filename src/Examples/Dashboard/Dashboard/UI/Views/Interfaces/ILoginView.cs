using System;

namespace Dashboard.UI.Views.Interfaces
{
	public interface ILoginView
	{
		event EventHandler Login;
		string Username { get; set; }
		string Password { get; set; }
		string ErrorMessage { set; }
	}
}