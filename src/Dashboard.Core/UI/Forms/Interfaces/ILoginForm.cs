using System;

namespace Dashboard.UI.Forms.Interfaces
{
	public interface ILoginForm
	{
		event EventHandler Login;
		string Username { get; set; }
		string Password { get; set; }
		string ErrorMessage { set; }
	}
}