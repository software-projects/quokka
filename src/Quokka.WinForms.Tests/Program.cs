using System;
using System.Windows.Forms;
using Quokka.WinForms.Startup;
using Quokka.WinForms.Testing;

namespace Quokka.WinForms.Tests
{
	public class Program : SplashScreenApplication
	{
		private SplashScreenPresenter _presenter;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Program());
		}

		protected override void OnSplashScreenDisplayed()
		{
			MainForm = new ViewTestForm();
		}
	}
}