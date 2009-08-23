using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Quokka.Krypton
{
	/// <summary>
	/// Property grid that paints itself using Krypton Toolkit properties.
	/// </summary>
	/// <remarks>
	/// Acknowledgement: This code adapted from code posted to the Krypton toolkit form by 'angel'.
	/// http://www.componentfactory.com/forums/viewtopic.php?f=7&t=1228
	/// </remarks>
	[ToolboxBitmap(typeof (PropertyGrid))]
	public class KryptonPropertyGrid : PropertyGrid
	{
		private IPalette _palette;
		private readonly PaletteRedirect _paletteRedirect;

		#region Properties

		private Color _gradientMiddleColor = Color.Gray;

		[Browsable(true), Category("Appearance-Extended")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("Color.Gray")]
		public Color GradientMiddleColor
		{
			get { return _gradientMiddleColor; }
			set
			{
				_gradientMiddleColor = value;
				Invalidate();
			}
		}

		#endregion

		#region Construction

		public KryptonPropertyGrid()
		{
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			UpdateStyles();

			// add Palette Handler
			if (_palette != null)
				_palette.PalettePaint += OnPalettePaint;

			KryptonManager.GlobalPaletteChanged += OnGlobalPaletteChanged;

			_palette = KryptonManager.CurrentGlobalPalette;
			_paletteRedirect = new PaletteRedirect(_palette);

			InitColors();
		}

		#endregion

		private void InitColors()
		{
			ToolStripRenderer = ToolStripManager.Renderer;
			_gradientMiddleColor = _palette.ColorTable.ToolStripGradientMiddle;
			HelpBackColor = _palette.ColorTable.MenuStripGradientBegin;
			HelpForeColor = _palette.ColorTable.StatusStripText;
			LineColor = _palette.ColorTable.ToolStripGradientMiddle;
			CategoryForeColor = _palette.ColorTable.StatusStripText;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.FillRectangle(new SolidBrush(_gradientMiddleColor), e.ClipRectangle);
			//
		}

		#region Krypton

		//Kripton Palette Events
		private void OnGlobalPaletteChanged(object sender, EventArgs e)
		{
			if (_palette != null)
				_palette.PalettePaint -= OnPalettePaint;

			_palette = KryptonManager.CurrentGlobalPalette;
			_paletteRedirect.Target = _palette;

			if (_palette != null)
			{
				_palette.PalettePaint += OnPalettePaint;
				//repaint with new values

				InitColors();
			}

			Invalidate();
		}

		//Kripton Palette Events
		private void OnPalettePaint(object sender, PaletteLayoutEventArgs e)
		{
			Invalidate();
		}

		#endregion
	}
}