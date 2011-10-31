using System;
using Quokka.UI.Commands;

namespace Dashboard.UI.Views.Interfaces
{
	public interface ILoginView
	{
		IUICommand LoginCommand { get; }
		string Username { get; set; }
		string Password { get; set; }
		string ErrorMessage { set; }
	}
}