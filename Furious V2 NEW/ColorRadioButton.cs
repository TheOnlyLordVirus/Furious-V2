using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Furious
{
    public class ColorRadioButton : RadioButton
    {
        // Fields
        private Color m_OnColour;
        private Color m_OffColour;
        private Rectangle m_circle;

        // Properties
        public Color OnColour
        {
            get
            {
                return m_OnColour;
            }
            set
            {
                if ((value == Color.White) || (value == Color.Transparent))
                    m_OnColour = Color.Empty;
                else
                    m_OnColour = value;
            }
        }
        public Color OffColour
        {
            get
            {
                return m_OffColour;
            }
            set
            {
                if ((value == Color.White) || (value == Color.Transparent))
                    m_OffColour = Color.Empty;
                else
                    m_OffColour = value;
            }
        }

        // Constructor
        public ColorRadioButton()
        {
            // Init
            m_circle = new Rectangle(1, 5, 10, 10 /*Magic Numbers*/);

            // Allows for Overlaying
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        // Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            // Init
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Overlay Graphic
            if (this.Checked)
            {
                if (OnColour != Color.Empty)
                {
                    g.FillEllipse(new SolidBrush(OnColour), m_circle);
                }
            }
            else
            {
                if (OffColour != Color.Empty)
                {
                    g.FillEllipse(new SolidBrush(OffColour), m_circle);
                }
            }
        }
    }
}