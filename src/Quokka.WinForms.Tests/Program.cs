using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quokka.WinForms.Tests
{
	using Quokka.WinForms.Testing;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ViewTestForm());
		}
	}
}