using System;
using System.Windows.Forms;
using Quokka.WinForms;
using Quokka.WinForms.Startup;

namespace Dashboard
{
	/// <summary>
	/// Program entry point.
	/// </summary>
	/// <remarks>
	/// This class is intended to show a splash screen as early as possible in the application
	/// startup. The splash screen is displayed even before most of the assemblies have been
	/// loaded. If you place any additional initialization code in this class, you might delay
	/// loading of the splash screen. Put initialization in the <see cref="Bootstrapper"/> class
	/// instead, as that gets loaded only after the splash screen is displayed.
	/// </remarks>
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
			ApplicationInfo.ProductName = "Example Program";

			// Default splashscreen for now.
			return base.CreateSplashScreen();
		}

		protected override void OnSplashScreenDisplayed()
		{
			MainForm = new Bootstrapper().Run();
		}
	}
}