using System;

namespace Quokka.WinForms.Startup
{
	public interface ISplashScreenView
	{
		Version Version { set; }
		string CompanyText { set; }
		string ProductText { set; }
		string CopyrightText { set; }
	}
}