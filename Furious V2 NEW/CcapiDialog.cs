using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS3Lib;

namespace Furious
{
    public partial class CcapiDialog : Form
    {
        public CcapiDialog()
        {
            InitializeComponent();
        }

        List<CCAPI.ConsoleInfo> cList = Form1.PS3.CCAPI.GetConsoleList();
        public static string ccapiIp = "1.1.1.1";
        public static bool tryConnect = false;

        private void CcapiDialog_Load(object sender, EventArgs e)
        {
            Form1.PS3.ChangeAPI(SelectAPI.ControlConsole);
            if (cList.Count != 0)
            {
                foreach (CCAPI.ConsoleInfo item in cList)
                {
                    listBox1.Items.Add(item.Name + " : " + item.Ip);
                }
            }
            if (listBox1.Items.Count != 0)
                listBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = cList[listBox1.SelectedIndex].Ip;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tryConnect = true;
            ccapiIp = textBox1.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tryConnect = false;
            Close();
        }

        public static int y;
        public static int x;
        public static Point newpoint = new Point();
        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            x = Control.MousePosition.X - Location.X;
            y = Control.MousePosition.Y - Location.Y;
        }
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newpoint = Control.MousePosition;
                newpoint.X -= x;
                newpoint.Y -= y;
                Location = newpoint;
            }
        }
    }
}
