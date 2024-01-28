
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PS3Lib;

namespace Furious.Theme;

public sealed class FuriousForm : Form
{
    public FuriousForm()
    {

    }

    #region Form
    const int WS_MINIMIZEBOX = 0x20000;
    const int CS_DBLCLKS = 0x8;
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style |= WS_MINIMIZEBOX;
            cp.ClassStyle |= CS_DBLCLKS;
            return cp;
        }
    }

    public bool DoAkimbo { get; private set; }

    public static PS3API PS3 = new PS3API();

    public class MyRenderer : ToolStripSystemRenderer
    {

        protected override void OnRenderSeparator(System.Windows.Forms.ToolStripSeparatorRenderEventArgs e)
        {
            using (System.Drawing.Drawing2D.LinearGradientBrush lgb = new System.Drawing.Drawing2D.LinearGradientBrush(Point.Empty, new Point(0, e.Item.Height), selectedTheme, selectedTabColor))
            {
                e.Graphics.FillRectangle(lgb, 0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);
            }
        }

        protected override void OnRenderMenuItemBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using (System.Drawing.Drawing2D.LinearGradientBrush lgb = new System.Drawing.Drawing2D.LinearGradientBrush(Point.Empty, new Point(0, e.Item.Height), selectedTheme, selectedTabColor))
                {
                    e.Graphics.FillRectangle(lgb, 0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);
                }
            }
            else
            {
                using (System.Drawing.Drawing2D.LinearGradientBrush lgb = new System.Drawing.Drawing2D.LinearGradientBrush(Point.Empty, new Point(0, e.Item.Height), selectedTheme, selectedTheme))
                {
                    e.Graphics.FillRectangle(lgb, 0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);
                }
            }
        }
    }

    private MyRenderer rndr = new MyRenderer();

    private void button12_Click(object sender, EventArgs e)
    {
        if (!setOnce)
            resetR1L1();
        Application.Exit();
    }

    private void button13_Click(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Minimized;
    }
    public static int y;
    public static int x;
    public static Point newpoint = new Point();
    private void panel2_MouseDown(object sender, MouseEventArgs e)
    {
        x = Control.MousePosition.X - Location.X;
        y = Control.MousePosition.Y - Location.Y;
    }

    private void panel2_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            newpoint = Control.MousePosition;
            newpoint.X -= x;
            newpoint.Y -= y;
            Location = newpoint;
        }
    }

    private void panel4_MouseDown(object sender, MouseEventArgs e)
    {
        x = Control.MousePosition.X - Location.X;
        y = Control.MousePosition.Y - Location.Y;
    }

    private void panel4_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            newpoint = Control.MousePosition;
            newpoint.X -= x;
            newpoint.Y -= y;
            Location = newpoint;
        }
    }

    private void panel3_MouseDown(object sender, MouseEventArgs e)
    {
        x = Control.MousePosition.X - Location.X;
        y = Control.MousePosition.Y - Location.Y;
    }

    private void panel3_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            newpoint = Control.MousePosition;
            newpoint.X -= x;
            newpoint.Y -= y;
            Location = newpoint;
        }
    }
    private void Form1_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            newpoint = Control.MousePosition;
            newpoint.X -= x;
            newpoint.Y -= y;
            Location = newpoint;
        }
    }

    private void Form1_MouseDown(object sender, MouseEventArgs e)
    {
        x = Control.MousePosition.X - Location.X;
        y = Control.MousePosition.Y - Location.Y;
    }

    private void button15_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Furious V2 RTM Tool - Modern Warfare 2\n\nVerison 2 contains new mods like client aimbot, client forge mode, spawn bots, spawn models and much more!\n\nTool developed by MayhemModding", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
    #region Form Load
    string listPath = Application.StartupPath + @"\modList.txt";
    private void Form1_Load(object sender, EventArgs e)
    {
        #region Theme
        try
        {
            Color theme = convertStr2Color(Properties.Settings.Default.saveColor);
            themeBox.Color = theme;
            toolTheme(theme);
        }
        catch
        {
            Color theme = Color.FromArgb(255, 10, 10);
            themeBox.Color = theme;
            toolTheme(theme);
        }
        int buttonY = 49;
        int buttonY2 = 29;
        int buttonX = -1;
        button2.Location = new Point(buttonX, buttonY * 0 + buttonY2);
        button3.Location = new Point(buttonX, buttonY * 1 + buttonY2);
        button4.Location = new Point(buttonX, buttonY * 2 + buttonY2);
        button5.Location = new Point(buttonX, buttonY * 3 + buttonY2);
        button6.Location = new Point(buttonX, buttonY * 4 + buttonY2);
        button7.Location = new Point(buttonX, buttonY * 5 + buttonY2);
        button8.Location = new Point(buttonX, buttonY * 6 + buttonY2);
        button9.Location = new Point(buttonX, buttonY * 7 + buttonY2);
        button10.Location = new Point(buttonX, buttonY * 8 + buttonY2);
        button11.Location = new Point(buttonX, buttonY * 9 + buttonY2 - 1);
        button2.ForeColor = selectedTheme;
        button2.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage1;
        tabControl1.Size = new Size(805, 540);
        clientStrip.Renderer = rndr;
        copyInfo.Renderer = rndr;
        #endregion
        #region datagrid
        for (int i = 0; i < statNames.Length; i++)
        {
            dataGridView1.RowCount += 1;
            dataGridView1[0, i].Value = statNames[i];
            dataGridView1[2, i].Value = "Set";
        }
        for (int i = 0; i < zoneDftText.Length; i++)
        {
            dataGridView4.RowCount += 1;
            dataGridView4[0, i].Value = zoneDftText[i];
            zoneColor[i] = "3F8000003F8000003F8000003F800000";
        }
        for (int i = 0; i < zoneColorDftText.Length; i++)
        {
            dataGridView5.RowCount += 1;
            dataGridView5[0, i].Value = zoneColorDftText[i];
            dataGridView5[1, i].Value = "Set";
            dataGridView5[2, i].Value = "Set";
            dataGridView5[3, i].Value = "Set";
        }

        dataGridView2.RowCount = 18;
        dataGridView3.RowCount = 18;
        for (int i = 0; i < 18; i++)
        {
            dataGridView2[0, i].Value = i;
            dataGridView3[0, i].Value = i;
        }
        #endregion
        #region misc
        for (int i = 0; i < 1; i++)
            target[i] = -1;

        colorComboBox2.SelectedIndex = 0;
        colorComboBox3.SelectedIndex = 0;
        colorComboBox7.SelectedIndex = 0;
        cardT = new string[richTextBox3.Lines.Count()];
        for (int i = 0; i < cardT.Length; i++)
            cardT[i] = richTextBox3.Lines[i];
        cardI = new string[richTextBox4.Lines.Count()];
        for (int i = 0; i < cardI.Length; i++)
            cardI[i] = richTextBox4.Lines[i];

        #endregion
    }
    #endregion
    #region Theme

    Color convertStr2Color(string color)
    {
        string[] split = Regex.Split(color, ",");
        return Color.FromArgb(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), Convert.ToInt32(split[2]));
    }

    ColorDialog themeBox = new ColorDialog();
    private void button14_Click(object sender, EventArgs e)
    {
        themeBox.FullOpen = true;
        if (themeBox.ShowDialog() == DialogResult.OK)
        {
            toolTheme(themeBox.Color);
            saveColor.Text = themeBox.Color.R.ToString() + "," + themeBox.Color.G.ToString() + "," + themeBox.Color.B.ToString();
            Properties.Settings.Default.saveColor = saveColor.Text;
            Properties.Settings.Default.Save();
        }
    }
    private void toolTheme(Color color)
    {
        selectedTheme = color;
        updateZoneTheme();
        BackColor = selectedTheme;
        panel2.BackColor = selectedTheme;
        panel3.BackColor = selectedTheme;
        panel4.BackColor = selectedTheme;
        foreach (Button control in Controls.OfType<Button>())
        {
            control.BackColor = selectedTheme;
        }
        foreach (Panel control1 in Controls.OfType<Panel>())
        {
            foreach (TabControl control2 in control1.Controls.OfType<TabControl>())
            {
                foreach (TabPage control3 in control2.Controls.OfType<TabPage>())
                {
                    changeTheme(control3);
                    foreach (System.Windows.Forms.GroupBox control5 in control3.Controls.OfType<GroupBox>())
                    {
                        changeTheme(control5);
                    }
                }
            }
        }
        button2.ForeColor = selectedTheme;
        button2.BackColor = selectedTabColor;
    }

    private void changeTheme(Control ctrl)
    {
        foreach (Button control in ctrl.Controls.OfType<Button>())
        {
            control.FlatAppearance.BorderColor = selectedTheme;
        }
        foreach (ColorRadioButton control in ctrl.Controls.OfType<ColorRadioButton>())
        {
            control.OnColour = selectedTheme;
            control.Refresh();
        }
        foreach (ColorCheckBox control in ctrl.Controls.OfType<ColorCheckBox>())
        {
            control.Refresh();
        }
        foreach (ColorComboBox control in ctrl.Controls.OfType<ColorComboBox>())
        {
            control.HighlightColor = selectedTheme;
        }
        foreach (DataGridView control in ctrl.Controls.OfType<DataGridView>())
        {
            control.RowsDefaultCellStyle.SelectionBackColor = selectedTheme;
        }
    }

    public static Color selectedTheme = Color.FromArgb(255, 10, 10);
    public static Color selectedTabColor = Color.FromArgb(30, 30, 30);

    private void resetTabColor()
    {
        button2.BackColor = selectedTheme;
        button3.BackColor = selectedTheme;
        button4.BackColor = selectedTheme;
        button5.BackColor = selectedTheme;
        button6.BackColor = selectedTheme;
        button7.BackColor = selectedTheme;
        button8.BackColor = selectedTheme;
        button9.BackColor = selectedTheme;
        button10.BackColor = selectedTheme;
        button11.BackColor = selectedTheme;
        button2.ForeColor = Color.Black;
        button3.ForeColor = Color.Black;
        button4.ForeColor = Color.Black;
        button5.ForeColor = Color.Black;
        button6.ForeColor = Color.Black;
        button7.ForeColor = Color.Black;
        button8.ForeColor = Color.Black;
        button9.ForeColor = Color.Black;
        button10.ForeColor = Color.Black;
        button11.ForeColor = Color.Black;
    }

    private void button2_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button2.ForeColor = selectedTheme;
        button2.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage1;
    }

    private void button11_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button11.ForeColor = selectedTheme;
        button11.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage10;
    }

    private void button10_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button10.ForeColor = selectedTheme;
        button10.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage9;
    }

    private void button9_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button9.ForeColor = selectedTheme;
        button9.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage8;
    }

    private void button8_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button8.ForeColor = selectedTheme;
        button8.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage7;
    }

    private void button7_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button7.ForeColor = selectedTheme;
        button7.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage6;
    }

    private void button6_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button6.ForeColor = selectedTheme;
        button6.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage5;
    }

    private void button5_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button5.ForeColor = selectedTheme;
        button5.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage4;
    }

    private void button4_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button4.ForeColor = selectedTheme;
        button4.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage3;
    }

    private void button3_Click(object sender, EventArgs e)
    {
        resetTabColor();
        button3.ForeColor = selectedTheme;
        button3.BackColor = selectedTabColor;
        tabControl1.SelectedTab = tabPage2;
    }
    #endregion
    #region Custom Color Ouput
    int[] toolRGB = new int[] { 255, 0, 0 };
    Color rainbowRGB(int[] changeRGB, int changeVal)
    {
        if (changeRGB[0] == 255 && changeRGB[1] < 255 && changeRGB[2] == 0)
            changeRGB[1] += changeVal;
        else if (changeRGB[0] > 0 && changeRGB[1] == 255 && changeRGB[2] == 0)
            changeRGB[0] -= changeVal;
        else if (changeRGB[0] == 0 && changeRGB[1] == 255 && changeRGB[2] < 255)
            changeRGB[2] += changeVal;
        else if (changeRGB[0] == 0 && changeRGB[1] > 0 && changeRGB[2] == 255)
            changeRGB[1] -= changeVal;
        else if (changeRGB[0] < 255 && changeRGB[1] == 0 && changeRGB[2] == 255)
            changeRGB[0] += changeVal;
        else if (changeRGB[0] == 255 && changeRGB[1] == 0 && changeRGB[2] > 0)
            changeRGB[2] -= changeVal;

        return Color.FromArgb(changeRGB[0], changeRGB[1], changeRGB[2]);
    }
    string RGB2HEX(Color rgb)
    {
        uint index = 0x3D00;
        double interval = 3.0117;
        double hexR = rgb.R * interval + index;
        double hexG = rgb.G * interval + index;
        double hexB = rgb.B * interval + index;
        double hexA = rgb.A * interval + index;
        string getHexR = String.Format("{0:X}", Convert.ToUInt32(hexR));
        string getHexG = String.Format("{0:X}", Convert.ToUInt32(hexG));
        string getHexB = String.Format("{0:X}", Convert.ToUInt32(hexB));
        string getHexA = String.Format("{0:X}", Convert.ToUInt32(hexA));
        return getHexR + "0000" + getHexG + "0000" + getHexB + "0000" + getHexA;
    }

    string RGB2CFG(Color rgb)
    {
        double convertR = 0.004 * rgb.R;
        double convertB = 0.004 * rgb.G;
        double convertG = 0.004 * rgb.B;
        double convertA = 0.004 * rgb.A;
        return convertR + " " + convertB + " " + convertG + " " + convertA;
    }

    private static byte[] ConvertHexToBytes(string input)
    {
        var result = new byte[(input.Length + 1) / 2];
        var offset = 0;
        if (input.Length % 2 == 1)
        {
            result[0] = (byte)Convert.ToUInt32(input[0] + "", 16);
            offset = 1;
        }
        for (int i = 0; i < input.Length / 2; i++)
        {
            result[i + offset] = (byte)Convert.ToUInt32(input.Substring(i * 2 + offset, 2), 16);
        }
        return result;
    }
    #endregion
}
