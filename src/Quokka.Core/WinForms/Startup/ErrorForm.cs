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
using Quokka.Properties;

namespace Quokka.WinForms.Startup
{
	/// <summary>
	/// Form that is displayed when an exception occurs very early in the program startup.
	/// </summary>
	/// <remarks>
	/// VERY IMPORTANT: This form should not depend on any third-party or any other assembly other
	/// than the .NET CLR assemblies. This means only controls from System.Windows.Forms. The problem
	/// may very well be that the third-party assembly is missing, and that would prevent this form
	/// from displaying.
	/// </remarks>
	public partial class ErrorForm : Form
	{
		public ErrorForm()
		{
			InitializeComponent();
		}

		public ErrorForm(Exception ex) : this()
		{
			SetError(ex);
		}

		public ErrorForm(string errorMessage) : this()
		{
			SetError(errorMessage);
		}

		public void SetError(object obj)
		{
			if (obj != null)
			{
				textBox.Text = obj.ToString();
			}
			textBox.Select(0, 0);
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		private void CopyLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Clipboard.SetText(textBox.Text);
			MessageBox.Show(this, Resources.DiagnosticInfoCopiedToClipboard, ApplicationInfo.MessageBoxCaption,
			                MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void FormLoad(object sender, EventArgs e)
		{
			Text = ApplicationInfo.ProductName;
			if (ApplicationInfo.Icon != null)
			{
				Icon = ApplicationInfo.Icon;
			}
		}
	}
}