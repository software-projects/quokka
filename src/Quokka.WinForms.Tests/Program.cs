using System;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using Quokka.Unity;
using Quokka.WinForms.Startup;
using Quokka.WinForms.Testing;

namespace Quokka.WinForms.Tests
{
	public class Program : SplashScreenApplication
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

		protected override void OnSplashScreenDisplayed()
		{
			IServiceLocator serviceLocator = ServiceContainerFactory.CreateContainer().Locator;
			ServiceLocator.SetLocatorProvider(() => serviceLocator);
			MainForm = new ViewTestForm();
		}
	}
}