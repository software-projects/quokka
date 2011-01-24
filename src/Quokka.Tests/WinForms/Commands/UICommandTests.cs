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
			var command = new UICommand(button);
			Assert.AreEqual(false, command.CanCheck);
			Assert.AreEqual(false, command.Checked);

			try
			{
				command.Checked = true;
				Assert.Fail("Expected NotSupportedException");
			}
			catch (NotSupportedException ex)
			{
				Assert.AreEqual("Checked property is not supported", ex.Message);
			}
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
	}
}