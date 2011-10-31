using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Quokka.Krypton
{
	public class KryptonUserControl : UserControl
	{
		private IPalette _palette;

		public KryptonUserControl()
		{
			InitializeComponent();
			KryptonManager.GlobalPaletteChanged += delegate { InitColors(); };
			InitColors();
		}

		private void InitColors()
		{
			_palette = KryptonManager.CurrentGlobalPalette; 
			BackColor = _palette.GetBackColor1(PaletteBackStyle.PanelClient, PaletteState.Normal);
			Invalidate();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// KryptonUserControl
			// 
			this.Name = "KryptonUserControl";
			this.Size = new System.Drawing.Size(382, 237);
			this.ResumeLayout(false);

		}
	}
}