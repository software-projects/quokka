using System;
using System.Windows.Forms;

namespace Quokka.WinForms.Config
{
	public class ListConfigGridView : DataGridView
	{
		public event EventHandler MoveUp;
		public event EventHandler EnterKeyPressed;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (EnterKeyPressed != null)
				{
					EnterKeyPressed(this, EventArgs.Empty);
					return;
				}
			}
			if (e.KeyData == Keys.Up)
			{
				var currentRow = CurrentRow;
				if (currentRow != null && currentRow.Index == 0)
				{
					if (MoveUp != null)
					{
						MoveUp(this, EventArgs.Empty);
						ClearSelection();
						return;
					}
				}
			}

			base.OnKeyDown(e);
		}
	}
}
