using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Quokka.WinForms
{
    [Designer(typeof (ViewManagerPanelControlDesigner), typeof (IDesigner))]
    [Docking(DockingBehavior.Ask)]
    //[ToolboxBitmap(typeof(ViewManagerPanel), "ViewManagerPanel")]
    public partial class ViewManagerPanel
    {
        private class ViewManagerPanelControlDesigner : ControlDesigner
        {
            // TODO: rewrite this to display something more interesting
            protected override void OnPaintAdornments(PaintEventArgs pe)
            {
                const int rectangleCount = 3;
                Bitmap bmp = new Bitmap(Control.Width, Control.Height);
                Graphics bmpGraphics = Graphics.FromImage(bmp);

                using (Pen pen = new Pen(Control.ForeColor))
                {
                    pen.DashStyle = DashStyle.Dash;
                    Rectangle clientRect = Control.ClientRectangle;
                    clientRect.Height -= 1;
                    clientRect.Width -= 1;

                    bmpGraphics.DrawRectangle(pen, clientRect);

                    pen.DashStyle = DashStyle.Solid;
                    int scaleFactor = 10;
                    int delta = scaleFactor*(rectangleCount + 1);
                    Size rectSize = new Size(clientRect.Width - delta, clientRect.Height - delta);

                    for (int i = 1; i <= rectangleCount; i++)
                    {
                        Rectangle rect =
                            new Rectangle(new Point(clientRect.X + scaleFactor*i, clientRect.Y + scaleFactor*i),
                                          rectSize);
                        bmpGraphics.DrawRectangle(pen, rect);
                        using (Brush fill = new SolidBrush(Control.BackColor))
                        {
                            bmpGraphics.FillRectangle(fill,
                                                      new Rectangle(++rect.X, ++rect.Y, --rect.Width, --rect.Height));
                        }
                    }
                }

                pe.Graphics.DrawImage(bmp, 0, 0);
            }
        }
    }
}