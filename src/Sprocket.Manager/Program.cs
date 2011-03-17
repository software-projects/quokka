using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Quokka.WinForms.Startup;
using Sprocket.Manager.Startup;
using Sprocket.Manager.Views;

namespace Sprocket.Manager
{
    internal class Program : SplashScreenApplication
    {
        /// <summary>
        ///   The main entry point for the application.
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
            return new SplashScreen();
        }

        protected override void OnSplashScreenDisplayed()
        {
            MainForm = new Bootstrapper().Run();
        }
    }
}
