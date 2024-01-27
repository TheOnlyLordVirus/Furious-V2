using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Furious_V2_NEW
{
    public class ColorCheckBox : CheckBox
    {
        public ColorCheckBox()
        {
            Appearance = System.Windows.Forms.Appearance.Button;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            TextAlign = ContentAlignment.TopLeft;
            FlatAppearance.BorderSize = 0;
            AutoSize = false;
            Height = 16;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //base.OnPaint(pevent);

            pevent.Graphics.Clear(BackColor);

            using (SolidBrush brush = new SolidBrush(ForeColor))
                pevent.Graphics.DrawString(Text, Font, brush, 21, 2);

            Point pt = new Point(4, 4);
            Rectangle rect = new Rectangle(pt, new Size(12, 12));

            pevent.Graphics.FillRectangle(Brushes.Beige, rect);

            if (Checked)
            {
                using (SolidBrush brush = new SolidBrush(Form1.selectedTheme))
                using (Font wing = new Font("Verdana", 10f, FontStyle.Regular))
                    pevent.Graphics.DrawString("■", wing, brush, 2, 3);
            }
            pevent.Graphics.DrawRectangle(Pens.DarkSlateBlue, rect);

            Rectangle fRect = ClientRectangle;

            if (Focused)
            {
                fRect.Inflate(-1, -1);
                using (Pen pen = new Pen(Brushes.Gray) { DashStyle = DashStyle.Dot })
                    pevent.Graphics.DrawRectangle(pen, fRect);
            }
        }
    }
}