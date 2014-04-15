#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Windows.Forms;

// NOTE: Do not add any references to classes from other assemblies.
// This class needs to load as quickly as possible during program startup.

namespace Quokka.WinForms.Startup
{
	public partial class DefaultSplashScreen : Form, ISplashScreenView
	{
		public DefaultSplashScreen()
		{
			InitializeComponent();
			Load += delegate
			        	{
			        		versionLabel.Text = String.Empty;
			        		buildLabel.Text = String.Empty;
			        		titleLabel.Text = Application.ProductName;
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
			set { titleLabel.Text = value; }
		}

		public string CopyrightText
		{
			set { copyrightLabel.Text = value; }
		}
	}
}