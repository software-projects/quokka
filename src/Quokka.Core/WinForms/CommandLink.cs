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

namespace Quokka.WinForms
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	/// <summary>
	/// Command link button that works with both Windows XP and Vista.
	/// </summary>
	public class CommandLink : Button
	{
		private readonly bool _supportedNatively;
		private bool _showShield;
		private bool _isMouseOver;
		private Color _textColor;
		private Color _mouseOverTextColor;
		private Color _focusButtonBorderColor;
		private Color _mouseOverBorderColor;
		private string _text;
		private Font _textFont;
		private Font _descriptionFont;
		private string _description;
		private Bitmap _arrowBitmap;
		private Bitmap _selectedArrowBitmap;
		private Bitmap _shieldBitmap;
		private Form _parentForm;
		private bool _isActivated;

		#region Construction, disposal

		public CommandLink()
		{
			// Check if the command link control is supported natively by the OS.
			// If so, let it do the implementing.
			_supportedNatively = Environment.OSVersion.Platform == PlatformID.Win32NT &&
			                     Environment.OSVersion.Version.Major >= 6;

			if (_supportedNatively) {
				FlatStyle = FlatStyle.System;
			}
			else {
				// Default text color
				_textColor = Color.FromArgb(21, 28, 85);
				_mouseOverTextColor = Color.FromArgb(7, 74, 229);

                // On Windows earlier than vista, it is not possible to guarantee that the 
                // Segoe UI font is installed. It would be good to include some code here to
                // see if that font is installed and use it if it is. (It is installed if Office 2007
                // is installed). Have not found a .NET API to tell which fonts are installed.
				_textFont = new Font("Tahoma", 12F, FontStyle.Bold);
				_descriptionFont = new Font("Tahoma", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
				_focusButtonBorderColor = Color.FromArgb(198, 244, 255);
				_mouseOverBorderColor = Color.FromArgb(198, 198, 198);
				base.BackColor = SystemColors.Window;
				FlatStyle = FlatStyle.Standard;
				TabStop = false;
				MouseEnter += CommandLink_MouseEnter;
				MouseLeave += CommandLink_MouseLeave;
			}

			// default size is bigger than a normal button
			Size = new Size(432, 72); 
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				if (_supportedNatively)
				{
					cp.Style |= Win32.BS_COMMANDLINK;
				}
				return cp;
			}
		}

		// Override this method to find out who the parent form is as
		// early as possible.
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			LocateParentForm();
		}

		#endregion

		#region Public properties

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		[DefaultValue("Text")]
		public override string Text
		{
			get { return _text; }
			set
			{
				if (_text != value) {
					_text = value;
					if (_supportedNatively) {
						base.Text = value;

                        // The text does not show on vista unless you explicitly
                        // set it like this.
                        Win32.SendMessage(this, Win32.WM_SETTEXT, 0, value);
					}
					else {
						Invalidate();
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		[DefaultValue("")]
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value) {
					_description = value;
					if (_supportedNatively) {
						Win32.SendMessage(this, Win32.BCM_SETNOTE, 0, value);
					}
					else {
						Invalidate();
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		[DefaultValue(false)]
		public bool ShowShield
		{
			get { return _showShield; }
			set
			{
				if (_showShield != value) {
					_showShield = value;
					if (_supportedNatively) {
						if (_showShield) {
							Win32.SendMessage(this, Win32.BCM_SETSHIELD, 0, 1);
						}
						else {
							Win32.SendMessage(this, Win32.BCM_SETSHIELD, 0, 0);
						}
					}
					else {
						Invalidate();
					}
				}
			}
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Is this the default button.
		/// </summary>
		/// <remarks>
		/// Relies on <see cref="LocateParentForm"/> for setting the parent form.
		/// </remarks>
		private bool IsDefaultButton
		{
			get
			{
				if (_parentForm == null)
					return false;
				return (this == _parentForm.AcceptButton);
			}
		}

		/// <summary>
		/// Bitmap for the arrow when not selected.
		/// </summary>
		private Bitmap ArrowBitmap
		{
			get
			{
				if (_arrowBitmap == null) {
					_arrowBitmap = new Bitmap(typeof(CommandLink), "Resources.Arrow.gif");
				}
				return _arrowBitmap;
			}
		}

		/// <summary>
		/// Bitmap for the arrow when the mouse is over.
		/// </summary>
		private Bitmap SelectedArrowBitmap
		{
			get
			{
				if (_selectedArrowBitmap == null) {
					_selectedArrowBitmap = new Bitmap(typeof(CommandLink), "Resources.SelectedArrow.gif");
				}
				return _selectedArrowBitmap;
			}
		}

		/// <summary>
		/// Bitmap for the shield.
		/// </summary>
		private Bitmap ShieldBitmap
		{
			get
			{
				if (_shieldBitmap == null) {
					_shieldBitmap = new Bitmap(typeof(CommandLink), "Resources.Shield.gif");
				}
				return _shieldBitmap;
			}
		}

		/// <summary>
		/// The bitmap to use for drawing the control.
		/// </summary>
		private Bitmap CurrentBitmap
		{
			get
			{
				if (_showShield) {
					return ShieldBitmap;
				}
				else if (_isMouseOver) {
					return SelectedArrowBitmap;
				}
				else {
					return ArrowBitmap;
				}
			}
		}

		/// <summary>
		/// Return brush for drawing main text.
		/// </summary>
		private Brush CreateTextBrush()
		{
			return new SolidBrush(_isMouseOver ? _mouseOverTextColor : _textColor);
		}

		/// <summary>
		/// Create a brush for the background.
		/// </summary>
		/// <returns>A <c>Brush</c> object, or <c>null</c> if no background brush.</returns>
		private Brush CreateBackgroundBrush()
		{
			if (_isMouseOver) {
				return new LinearGradientBrush(ClientRectangle,
				                               BackColor,
				                               Color.FromArgb(246, 246, 246), // TODO: make configurable
				                               System.Drawing.Drawing2D.LinearGradientMode.Vertical);
			}
			else {
				return null;
			}
		}

		/// <summary>
		/// Create a pen to draw the border.
		/// </summary>
		/// <returns>A <c>Pen</c> object, or <c>null</c> if no border to be drawn.</returns>
		private Pen CreateBorderPen()
		{
			Pen pen = null;

			if (_isMouseOver) {
				// Draw the border if the mouse is hovering over
				// TODO: This color should be configurable.
				pen = new Pen(_mouseOverBorderColor);
			}
			else if (Focused) {
				pen = new Pen(_focusButtonBorderColor);
			}
			else {
				// If no other control has focus and this is the default
				// button, then draw the same border as if it has focus.
				// TODO: This does not exactly mimic Windows Visa behaviour,
				// but it is probably close enough.
				Control activeControl = null;
				if (_parentForm != null) {
					activeControl = _parentForm.ActiveControl;
				}

				if (_isActivated && IsDefaultButton && activeControl == null) {
					pen = new Pen(_focusButtonBorderColor);
				}
			}

			return pen;
		}

		private static GraphicsPath CreateRoundedRectangle(Rectangle r)
		{
			const int radius = 5;
			const int diameter = radius * 2;
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(r.X + r.Width - diameter, r.Y, diameter, diameter, 270, 90);
			graphicsPath.AddArc(r.X + r.Width - diameter, r.Y + r.Height - diameter, diameter, diameter, 0, 90);
			graphicsPath.AddArc(r.X, r.Y + r.Height - diameter, diameter, diameter, 90, 90);
			graphicsPath.AddArc(r.X, r.Y, diameter, diameter, 180, 90);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_supportedNatively) {
				base.OnPaint(e);
			}
			else {
				// TODO: There seems to be no way to handle an event to check if the
				// control's top-level form has changed, so we need to check it each time
				// we paint. It seems a bit inefficient.
				LocateParentForm();

				Rectangle r = ClientRectangle;
				r.Inflate(-1, -1);

				// draw the background
				// TODO: should this be done in WM_ERASEBACKGROUND?
				using (Brush brush = new SolidBrush(BackColor)) {
					e.Graphics.FillRectangle(brush, ClientRectangle);
				}

				using (GraphicsPath graphicsPath = CreateRoundedRectangle(r)) {
					// draw the background
					using (Brush brush = CreateBackgroundBrush()) {
						if (brush != null) {
							//e.Graphics.FillRectangle(brush, ClientRectangle);
							e.Graphics.FillPath(brush, graphicsPath);
						}
					}

					// draw the bitmap
					Bitmap bitmap = CurrentBitmap;
					if (bitmap != null) {
						e.Graphics.DrawImage(bitmap, 10, 13);
					}

					// draw the text
					using (Brush textBrush = CreateTextBrush()) {
						e.Graphics.DrawString(_text, _textFont, textBrush, 27, 10);
						e.Graphics.DrawString(_description, _descriptionFont, textBrush, 32, 36);
					}

					// draw the border
					using (Pen pen = CreateBorderPen()) {
						if (pen != null) {
							e.Graphics.DrawPath(pen, graphicsPath);
						}
					}
				}
			}
		}

		#endregion

		# region Other private methods

		/// <summary>
		/// Locate the parent form for this control, and subscribe to required events.
		/// </summary>
		private void LocateParentForm()
		{
			Form parentForm = FindForm();
			if (parentForm != _parentForm)
			{
				if (_parentForm != null)
				{
					_parentForm.Activated -= ParentForm_Activated;
					_parentForm.Deactivate -= ParentForm_Deactivate;
				}
				_parentForm = parentForm;
				if (_parentForm != null)
				{
					_parentForm.Activated += ParentForm_Activated;
					_parentForm.Deactivate += ParentForm_Deactivate;
					_isActivated = (_parentForm == Form.ActiveForm);
				}
			}
		}

		/// <summary>
		/// Check if the mouse is over the button.
		/// </summary>
		private void CheckIfMouseOver()
		{
			Point p = PointToClient(MousePosition);
			bool mouseOver = ClientRectangle.Contains(p);
			if (mouseOver != _isMouseOver)
			{
				_isMouseOver = mouseOver;
				Invalidate();
			}
		}

		#endregion

		#region Event Handlers

		private void CommandLink_MouseEnter(object sender, EventArgs e)
		{
			CheckIfMouseOver();
		}

		private void CommandLink_MouseLeave(object sender, EventArgs e)
		{
			CheckIfMouseOver();
		}

		private void ParentForm_Activated(object sender, EventArgs e)
		{
			_isActivated = true;
			CheckIfMouseOver();
			Invalidate();
		}

		private void ParentForm_Deactivate(object sender, EventArgs e)
		{
			_isActivated = false;
			_isMouseOver = false;
			Invalidate();
		}

		#endregion
	}
}