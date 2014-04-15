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
using ComponentFactory.Krypton.Toolkit;
using NUnit.Framework;
using Quokka.DynamicCodeGeneration;
using Quokka.WinForms.Internal;

namespace Quokka.WinForms.Commands
{
	[TestFixture]
	public class UICommandTests
	{
		[Test]
		public void ButtonText()
		{
			var button = new Button {Text = "Button text"};
			var command = new UICommand(button);
			string propertyName = null;
			command.PropertyChanged += (sender, e) => propertyName = e.PropertyName;
			Assert.AreEqual("Button text", command.Text);

			button.Text = "Text1";
			Assert.AreEqual("Text1", command.Text);
			Assert.AreEqual("Text", propertyName);
		}

		[Test]
		public void ButtonEnabled()
		{
			var button = new Button {Enabled = true};
			var command = new UICommand(button);
			string propertyName = null;
			command.PropertyChanged += (sender, e) => propertyName = e.PropertyName;
			Assert.AreEqual(true, command.Enabled);

			button.Enabled = false;

			Assert.AreEqual(false, command.Enabled);
			Assert.AreEqual("Enabled", propertyName);
		}

		[Test]
		public void ButtonCheckNotSupported()
		{
			var button = new Button();
			var checkBoxInterface = ProxyFactory.CreateDuckProxy<ICheckControl>(button);

			Assert.AreEqual(false, checkBoxInterface.IsCheckedSupported);
			Assert.AreEqual(false, checkBoxInterface.IsCheckedChangedSupported);

			var command = new UICommand(button);
			Assert.AreEqual(false, command.Checked);
			bool checkChangedRaised = false;
			command.PropertyChanged += (o, e) =>
			                           	{
			                           		if (e.PropertyName == "Checked")
			                           		{
			                           			checkChangedRaised = true;
			                           		}
			                           	};
			command.Checked = true;
			Assert.IsTrue(checkChangedRaised);
		}

		[Test]
		public void ButtonCheckSupported()
		{
			var checkBox = new CheckBox {Checked = true};
			var checkBoxInterface = ProxyFactory.CreateDuckProxy<ICheckControl>(checkBox);

			Assert.AreEqual(true, checkBoxInterface.IsCheckedSupported);
			Assert.AreEqual(true, checkBoxInterface.Checked);

			Assert.AreEqual(true, checkBoxInterface.IsCheckedChangedSupported);

			bool checkChangedRaised = false;
			checkBoxInterface.CheckedChanged += delegate { checkChangedRaised = true; };
			checkBox.Checked = false;
			Assert.AreEqual(false, checkBoxInterface.Checked);
			Assert.IsTrue(checkChangedRaised);
		}

		[Test]
		public void KryptonButtonCheckSupported()
		{
			var checkBox = new KryptonCheckBox {Checked = true};
			var checkBoxInterface = ProxyFactory.CreateDuckProxy<ICheckControl>(checkBox);

			Assert.AreEqual(true, checkBoxInterface.IsCheckedSupported);
			Assert.AreEqual(true, checkBoxInterface.Checked);

			Assert.AreEqual(true, checkBoxInterface.IsCheckedChangedSupported);

			bool checkChangedRaised = false;
			checkBoxInterface.CheckedChanged += delegate { checkChangedRaised = true; };
			checkBox.Checked = false;
			Assert.AreEqual(false, checkBoxInterface.Checked);
			Assert.IsTrue(checkChangedRaised);
		}

		[Test]
		public void NoControl()
		{
			var command = new UICommand();

			Assert.IsFalse(command.Checked);
			Assert.IsFalse(command.Enabled);
			Assert.AreEqual(string.Empty, command.Text);

			bool enabledChanged = false;
			bool textChanged = false;
			bool checkedChanged = false;

			command.PropertyChanged += (o, e) => {
				switch (e.PropertyName)
				{
					case "Checked":
						checkedChanged = true;
						break;
					case "Enabled":
						enabledChanged = true;
						break;
					case "Text":
						textChanged = true;
						break;
					default:
						Assert.Fail("Unknown property name: " + e.PropertyName);
						break;
				}
			};

			command.Enabled = true;
			Assert.IsTrue(enabledChanged);
			Assert.IsFalse(textChanged);
			Assert.IsFalse(checkedChanged);
			enabledChanged = false;

			command.Checked = true;
			Assert.IsFalse(enabledChanged);
			Assert.IsFalse(textChanged);
			Assert.IsTrue(checkedChanged);
			checkedChanged = false;

			command.Text = "X";
			Assert.IsFalse(enabledChanged);
			Assert.IsTrue(textChanged);
			Assert.IsFalse(checkedChanged);
			textChanged = false;
		}
	}
}