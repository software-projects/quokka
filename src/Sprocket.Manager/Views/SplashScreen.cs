using System;
using System.Windows.Forms;
using Quokka.WinForms.Startup;

// NOTE: Do not add any references to classes from other assemblies.
// This class needs to load as quickly as possible during program startup.

namespace Sprocket.Manager.Views
{
	public partial class SplashScreen : Form, ISplashScreenView
	{
		public SplashScreen()
		{
			InitializeComponent();
			Load += delegate
			        	{
			        		versionLabel.Text = String.Empty;
			        		buildLabel.Text = String.Empty;
			        		//titleLabel.Text = Application.ProductName;
			        		copyrightLabel.Text = "Loading application ...";
			        	};
		}

		public Version Version
		{
			set
			{
				if (value == null)
				{
					value = new Version();
				}

				if (value.Major != 0 || value.Minor != 0)
				{
					versionLabel.Text = String.Format("Version {0}.{1}", value.Major, value.Minor);
				}

				if (value.Build != 0 || value.Revision != 0)
				{
					buildLabel.Text = String.Format("Build {0} Rev {1}", value.Build, value.Revision);
				}
			}
		}

		public string CompanyText
		{
			set { /* not used yet */ }
		}

		public string ProductText
		{
			set {  }
		}

		public string CopyrightText
		{
			set { copyrightLabel.Text = value; }
		}

	}
}