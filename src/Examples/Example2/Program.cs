using System;
using System.Windows.Forms;
using Quokka.WinForms.Startup;

namespace Example2
{
	internal class Program : SplashScreenApplication
	{
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

		protected override Form CreateSplashScreen()
		{
			// TODO: can create a custom splash screen here, return
			// null for the default.
			return null;
		}

		protected override void OnSplashScreenDisplayed()
		{
			MainForm = new Bootstrapper().Run();
		}
	}
}