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
using System.Drawing;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Config.Storage;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Config
{
	public partial class StringEditor : UserControl, IConfigParameterEditor
	{
		private Control _userControlParent;

		/// <summary>
		/// Desired width in characters.
		/// </summary>
		public int CharCount { get; set; }

		public StringEditor()
		{
			InitializeComponent();
			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
			if (CharCount > 0)
			{
				textBox.Width = TextRenderer.MeasureText(new string('m', CharCount), textBox.Font).Width;
			}
			else
			{
				// size not specified, use all available space
				var parent = Parent;
				while (parent != null && !(parent is UserControl))
				{
					parent = parent.Parent;
				}

				if (parent != null)
				{
					parent.SizeChanged += UserControlSizeChanged;
					_userControlParent = parent;
				}				
			}
		}

		public Control Control
		{
			get { return this; }
		}

		public IConfigParameter Parameter { get; private set; }

		public void Initialize(IConfigParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			Parameter = parameter;
			textBox.Text = parameter.GetValueText();
		}

		public string TextValue
		{
			get { return textBox.Text; }
		}

		private void UserControlSizeChanged(object sender, EventArgs e)
		{
			if (_userControlParent != null)
			{
				var parentLocation = _userControlParent.PointToScreen(new Point(0, 0));
				var textBoxLocation = textBox.PointToScreen(new Point(0, 0));
				var margin = textBoxLocation.X - parentLocation.X;
				// TODO: need to use system metrics for this
				textBox.Width = _userControlParent.Width - margin - 40;
			}
		}
	}
}