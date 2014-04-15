using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.WinForms.Internal
{
	public interface ICheckControl
	{
		bool Checked { get; set; }
		bool IsCheckedSupported { get; }

		event EventHandler CheckedChanged;
		bool IsCheckedChangedSupported { get; }
	}

	public interface ICommandControl : ICheckControl
	{
		string Text { get; set; }
		bool Enabled { get; set; }

	}
}
