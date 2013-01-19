using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quokka.WinForms.Config
{
	public class SearchTextBox : TextBox
	{
		public event EventHandler DownKeyPressed;
		public event EventHandler EnterKeyPressed;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter && EnterKeyPressed != null)
			{
				EnterKeyPressed(this, EventArgs.Empty);
				return;
			}
			if (e.KeyData == Keys.Down && DownKeyPressed != null)
			{
				DownKeyPressed(this, EventArgs.Empty);
				return;
			}
			base.OnKeyDown(e);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Text = string.Empty;
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
}
