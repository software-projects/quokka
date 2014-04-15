using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Quokka.WinForms
{
    [Flags]
    public enum CornerCurveMode
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        TopLeft_TopRight = 3,
        BottomLeft = 4,
        TopLeft_BottomLeft = 5,
        TopRight_BottomLeft = 6,
        TopLeft_TopRight_BottomLeft = 7,
        BottomRight = 8,
        BottomRight_TopLeft = 9,
        BottomRight_TopRight = 10,
        BottomRight_TopLeft_TopRight = 11,
        BottomRight_BottomLeft = 12,
        BottomRight_TopLeft_BottomLeft = 13,
        BottomRight_TopRight_BottomLeft = 14,
        All = 15
    }

    public enum MyLinearGradientMode
    {
        Horizontal = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3,
        None = 4,
    }

    [ToolboxBitmap(typeof(Panel))]
    public class CustomPanel : Panel
    {
        #region Fields
        private Color _BackColour1 = SystemColors.Window;
        private Color _BackColour2 = SystemColors.Window;
        private MyLinearGradientMode _GradientMode = MyLinearGradientMode.None;
        private BorderStyle _BorderStyle = BorderStyle.None;
        private Color _BorderColour = SystemColors.WindowFrame;
        private int _BorderWidth = 1;
        private int _Curvature = 0;
        private CornerCurveMode _CurveMode = CornerCurveMode.All;
        #endregion

        #region Properties
        [DefaultValue(typeof(Color), "Window")]
        [Category("Appearance")]
        [Description("The primary background color used to display text and graphics in the control.")]
        public new Color BackColor {
            get { return this._BackColour1; }
            set {
                this._BackColour1 = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "Window")]
        [Category("Appearance")]
        [Description("The secondary background color used to paint the control.")]
        public Color BackColor2 {
            get { return this._BackColour2; }
            set {
                this._BackColour2 = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(MyLinearGradientMode), "None")]
        [Category("Appearance")]
        [Description("The gradient direction used to paint the control.")]
        public MyLinearGradientMode GradientMode {
            get { return this._GradientMode; }
            set {
                this._GradientMode = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(BorderStyle), "None")]
        [Category("Appearance")]
        [Description("The border style used to paint the control.")]
        public new BorderStyle BorderStyle {
            get { return this._BorderStyle; }
            set {
                this._BorderStyle = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "WindowFrame")]
        [Category("Appearance")]
        [Description("The border color used to paint the control.")]
        public Color BorderColor {
            get { return this._BorderColour; }
            set {
                this._BorderColour = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(int), "1")]
        [Category("Appearance")]
        [Description("The width of the border used to paint the control.")]
        public int BorderWidth {
            get { return this._BorderWidth; }
            set {
                this._BorderWidth = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(int), "0")]
        [Category("Appearance")]
        [Description("The radius of the curve used to paint the corners of the control.")]
        public int Curvature {
            get { return this._Curvature; }
            set {
                this._Curvature = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(CornerCurveMode), "All")]
        [Category("Appearance")]
        [Description("The style of the curves to be drawn on the control.")]
        public CornerCurveMode CurveMode {
            get { return this._CurveMode; }
            set {
                this._CurveMode = value;
                if (DesignMode) {
                    Invalidate();
                }
            }
        }

        public int AdjustedCurve {
            get {
                int curve = 0;
                if (this._CurveMode != CornerCurveMode.None) {
                    if (this._Curvature > (ClientRectangle.Width / 2)) {
                        curve = DoubleToInt(ClientRectangle.Width / 2);
                    }
                    else {
                        curve = this._Curvature;
                    }

                    if (curve > (ClientRectangle.Height / 2)) {
                        curve = DoubleToInt(ClientRectangle.Height / 2);
                    }
                }
                return curve;
            }
        }

        #endregion

        #region Construction/Disposal
        public CustomPanel() {
            SetDefaultControlStyles();
            CustomInitialisation();
        }
        #endregion

        #region Methods

        private void SetDefaultControlStyles() {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.ContainerControl, False);
        }

        private void CustomInitialisation() {
            SuspendLayout();
            base.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.None;
            ResumeLayout(false);
        }

        private static int DoubleToInt(double value) {
            return Decimal.ToInt32(Decimal.Floor(Decimal.Parse(value.ToString())));
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            GraphicsPath graphPath = this.GetPath();

            // Create gradient brush (cannot be width or height 0)
            LinearGradientBrush filler;
            Rectangle rect = this.ClientRectangle;

            if (this.ClientRectangle.Width == 0) {
                rect.Width += 1;
            }

            if (this.ClientRectangle.Height == 0) {
                rect.Height += 1;
            }

            if (this._GradientMode == MyLinearGradientMode.None) {
                filler = new LinearGradientBrush(rect, this._BackColour1, this._BackColour1, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            }
            else {
                filler = new LinearGradientBrush(rect, this._BackColour1, this._BackColour2, ConvertGradientMode(this._GradientMode));
            }

            e.Graphics.FillPath(filler, graphPath);
            filler.Dispose();

            switch (this._BorderStyle) {
                case BorderStyle.FixedSingle:
                    using (Pen borderPen = new Pen(this._BorderColour, this._BorderWidth)) {
                        e.Graphics.DrawPath(borderPen, graphPath);
                    }
                    break;

                case BorderStyle.Fixed3D:
                    this.DrawBorder3D(e.Graphics, this.ClientRectangle);
                    break;

                case BorderStyle.None:
                    // do nothing
                    break;
            }
        }

        private void DrawBorder3D(Graphics graphics, Rectangle rectangle) {
            graphics.SmoothingMode = SmoothingMode.Default;
            graphics.DrawLine(SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Y);
            graphics.DrawLine(SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.X, rectangle.Height - 1);
            graphics.DrawLine(SystemPens.ControlDarkDark, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 1, rectangle.Y + 1);
            graphics.DrawLine(SystemPens.ControlDarkDark, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Height - 1);
            graphics.DrawLine(SystemPens.ControlLight, rectangle.X + 1, rectangle.Height - 2, rectangle.Width - 2, rectangle.Height - 2);
            graphics.DrawLine(SystemPens.ControlLight, rectangle.Width - 2, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);
            graphics.DrawLine(SystemPens.ControlLightLight, rectangle.X, rectangle.Height - 1, rectangle.Width - 1, rectangle.Height - 1);
            graphics.DrawLine(SystemPens.ControlLightLight, rectangle.Width - 1, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
        }

        private System.Drawing.Drawing2D.LinearGradientMode ConvertGradientMode(MyLinearGradientMode linearGradientMode) {
            switch (linearGradientMode) {
                case MyLinearGradientMode.BackwardDiagonal:
                    return System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
                case MyLinearGradientMode.ForwardDiagonal:
                    return System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
                case MyLinearGradientMode.Horizontal:
                    return System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
                case MyLinearGradientMode.Vertical:
                default:
                    return System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            }
        }

        private GraphicsPath GetPath() {
            GraphicsPath graphPath = new GraphicsPath();

            if (this._BorderStyle == BorderStyle.Fixed3D) {
                graphPath.AddRectangle(this.ClientRectangle);
            }
            else {
                try {
                    int curve = 0;
                    Rectangle rect = this.ClientRectangle;
                    int offset = 0;

                    switch (this._BorderStyle) {
                        case BorderStyle.FixedSingle:
                            if (this._BorderWidth > 1) {
                                offset = DoubleToInt(this.BorderWidth / 2);
                            }
                            curve = this.AdjustedCurve;
                            break;

                        case BorderStyle.Fixed3D: // should not happen
                        case BorderStyle.None:
                            curve = this.AdjustedCurve;
                            break;
                    }

                    if (curve == 0) {
                        graphPath.AddRectangle(Rectangle.Inflate(rect, -offset, -offset));
                    }
                    else {
                        int rectWidth = rect.Width - 1 - offset;
                        int rectHeight = rect.Height - 1 - offset;
                        int curveWidth = 1;

                        if ((this._CurveMode & CornerCurveMode.TopRight) != 0) {
                            curveWidth = (curve * 2);
                        }
                        else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(rectWidth - curveWidth, offset, curveWidth, curveWidth, 270, 90);

                        if ((this._CurveMode & CornerCurveMode.BottomRight) != 0) {
                            curveWidth = (curve * 2);
                        }
                        else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(rectWidth - curveWidth, rectHeight - curveWidth, curveWidth, curveWidth, 0, 90);

                        if ((this._CurveMode & CornerCurveMode.BottomLeft) != 0) {
                            curveWidth = (curve * 2);
                        }
                        else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(offset, rectHeight - curveWidth, curveWidth, curveWidth, 90, 90);

                        if ((this._CurveMode & CornerCurveMode.TopLeft) != 0) {
                            curveWidth = (curve * 2);
                        }
                        else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(offset, offset, curveWidth, curveWidth, 180, 90);

                        graphPath.CloseFigure();
                    }
                }
                catch (Exception) {
                    graphPath.AddRectangle(this.ClientRectangle);
                }
            }
            return graphPath;
        }

        #endregion
    }
}
