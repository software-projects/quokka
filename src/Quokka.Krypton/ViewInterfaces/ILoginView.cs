using System;

namespace Quokka.Krypton.ViewInterfaces
{
	public interface ILoginView
	{
		event EventHandler Login;
		string Username { get; set; }
		string Password { get; set; }
		string ErrorMessage { set; }
	}
}